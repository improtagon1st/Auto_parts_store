﻿using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Word = Microsoft.Office.Interop.Word;
using WinForms = System.Windows.Forms;
using Charting = System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Threading.Tasks;

namespace Auto_parts_store
{
    public partial class ReportsPage : System.Windows.Controls.Page
    {
        private readonly MainWindow _mainWindow;
        private readonly Users _currentUser;

        public ReportsPage(MainWindow mainWindow, Users currentUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = currentUser;
        }

        private async System.Threading.Tasks.Task GenerateStockAsync(string path) => await System.Threading.Tasks.Task.Run(() =>
        {
            var ctx = Entities.GetContext();
            var data = ctx.AutoParts.Select(p => new
            {
                p.PartID,
                p.PartName,
                p.StockQuantity,
                p.Price,
                Total = p.StockQuantity * p.Price
            }).ToList();

            using (var package = CreateOrOpen(path))
            {
                var ws = FreshSheet(package, "Остатки");
                ws.Cells[1, 1].Value = "№ автозапчасти";
                ws.Cells[1, 2].Value = "Наименование";
                ws.Cells[1, 3].Value = "Остаток";
                ws.Cells[1, 4].Value = "Цена";
                ws.Cells[1, 5].Value = "Сумма";

                int r = 2;
                foreach (var x in data)
                {
                    ws.Cells[r, 1].Value = x.PartID;
                    ws.Cells[r, 2].Value = x.PartName;
                    ws.Cells[r, 3].Value = x.StockQuantity;
                    ws.Cells[r, 4].Value = x.Price;
                    ws.Cells[r, 5].Value = x.Total;
                    r++;
                }
                ws.Cells[1, 1, r - 1, 5].AutoFitColumns();
                package.Save();
            }
        }).ContinueWith(t => ShowStatus(t, "Остатки"));

        private async System.Threading.Tasks.Task GenerateOpsAsync(string path) => await System.Threading.Tasks.Task.Run(() =>
        {
            var ctx = Entities.GetContext();
            var data = ctx.PurchaseOrderDetails.Select(d => new
            {
                d.PurchaseOrderDetailID,
                Supplier = d.PurchaseOrders.Suppliers.SupplierName,
                PartName = d.AutoParts.PartName,
                d.Quantity,
                Sum = d.Quantity * d.UnitPrice
            }).ToList();

            using (var package = CreateOrOpen(path))
            {
                var ws = FreshSheet(package, "Операции");
                ws.Cells[1, 1].Value = "№ операции";
                ws.Cells[1, 2].Value = "Поставщик";
                ws.Cells[1, 3].Value = "Запчасть";
                ws.Cells[1, 4].Value = "Кол-во";
                ws.Cells[1, 5].Value = "Сумма";

                int r = 2;
                foreach (var x in data)
                {
                    ws.Cells[r, 1].Value = x.PurchaseOrderDetailID;
                    ws.Cells[r, 2].Value = x.Supplier;
                    ws.Cells[r, 3].Value = x.PartName;
                    ws.Cells[r, 4].Value = x.Quantity;
                    ws.Cells[r, 5].Value = x.Sum;
                    r++;
                }
                ws.Cells[1, 1, r - 1, 5].AutoFitColumns();
                package.Save();
            }
        }).ContinueWith(t => ShowStatus(t, "Операции"));

        private void ShowChart_Click(object sender, RoutedEventArgs e)
        {
            var chart = new Charting.Chart();
            chart.ChartAreas.Add(new Charting.ChartArea("MainArea"));
            chart.Series.Add("Остатки");
            chart.Series["Остатки"].ChartType = Charting.SeriesChartType.Column;

            var ctx = Entities.GetContext();
            var parts = ctx.AutoParts.OrderByDescending(p => p.StockQuantity).Take(10).ToList();

            foreach (var part in parts)
            {
                chart.Series["Остатки"].Points.AddXY(part.PartName, part.StockQuantity);
            }

            chart.Dock = WinForms.DockStyle.Fill;
            ChartHost.Child = chart;
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            var ctx = Entities.GetContext();
            var parts = ctx.AutoParts.ToList();

            var app = new Word.Application();
            var doc = app.Documents.Add();

            Word.Paragraph title = doc.Paragraphs.Add();
            title.Range.Text = "Отчёт по автозапчастям";
            title.Range.set_Style("Заголовок 1");
            title.Range.InsertParagraphAfter();

            Word.Table table = doc.Tables.Add(title.Range, parts.Count + 1, 3);
            table.Borders.Enable = 1;
            table.Cell(1, 1).Range.Text = "Название";
            table.Cell(1, 2).Range.Text = "Количество";
            table.Cell(1, 3).Range.Text = "Цена";

            for (int i = 0; i < parts.Count; i++)
            {
                table.Cell(i + 2, 1).Range.Text = parts[i].PartName;
                table.Cell(i + 2, 2).Range.Text = parts[i].StockQuantity.ToString();
                table.Cell(i + 2, 3).Range.Text = parts[i].Price.ToString("C");
            }

            app.Visible = true;
        }

        private static ExcelPackage CreateOrOpen(string path) => new ExcelPackage(new FileInfo(path));

        private static ExcelWorksheet FreshSheet(ExcelPackage pkg, string name)
        {
            var sheet = pkg.Workbook.Worksheets[name];
            if (sheet != null) pkg.Workbook.Worksheets.Delete(sheet);
            return pkg.Workbook.Worksheets.Add(name);
        }

        private bool ChooseFile(string defaultName, out string file)
        {
            var dlg = new SaveFileDialog
            {
                FileName = defaultName,
                Filter = "Excel (*.xlsx)|*.xlsx",
                DefaultExt = ".xlsx"
            };
            bool ok = dlg.ShowDialog() == true;
            file = ok ? dlg.FileName : null;
            return ok;
        }

        private async void StockReport_Click(object sender, RoutedEventArgs e)
        {
            if (!ChooseFile("Текущие остатки.xlsx", out string file)) return;
            await GenerateStockAsync(file);
        }

        private async void OperationsReport_Click(object sender, RoutedEventArgs e)
        {
            if (!ChooseFile("Выполненные операции.xlsx", out string file)) return;
            await GenerateOpsAsync(file);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new ManagerPage(_mainWindow, _currentUser));
        }
        private void ShowStatus(System.Threading.Tasks.Task t, string what)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = t.Exception == null
                    ? $"Отчёт «{what}» готов"
                    : $"Ошибка: {t.Exception.GetBaseException().Message}";
            });
        }
    }
}
