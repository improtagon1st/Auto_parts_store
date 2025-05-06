using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace Auto_parts_store
{
    public partial class AddPurchaseOrderPage : Page
    {
        private readonly MainWindow _mw;
        private readonly Users _currentUser;

        private readonly List<OrderItem> _items = new List<OrderItem>();

        public AddPurchaseOrderPage(MainWindow mw, Users user)
        {
            InitializeComponent();
            _mw = mw;
            _currentUser = user;

            LoadCombos();
            RefreshGrid();
        }

        private void LoadCombos()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                SupplierCombo.ItemsSource = db.Suppliers.ToList();
                PartCombo.ItemsSource = db.AutoParts.ToList();
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierCombo.SelectedItem == null)
            {
                MessageBox.Show("Сначала выберите поставщика.");
                return;
            }

            var part = PartCombo.SelectedItem as AutoParts;
            if (part == null)
            {
                MessageBox.Show("Выберите запчасть.");
                return;
            }

            int qty;
            if (!int.TryParse(QtyBox.Text, out qty) || qty <= 0)
            {
                MessageBox.Show("Количество должно быть положительным числом.");
                return;
            }

            var exist = _items.FirstOrDefault(i => i.PartID == part.PartID);
            if (exist != null)
                exist.Quantity += qty;
            else
                _items.Add(new OrderItem
                {
                    PartID = part.PartID,
                    PartName = part.PartName,
                    UnitPrice = part.Price,
                    Quantity = qty
                });

            QtyBox.Clear();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            OrderItemsGrid.ItemsSource = null;
            OrderItemsGrid.ItemsSource = _items;

            TotalText.Text = "Итого: " +
                             _items.Sum(i => i.Total).ToString("0.00") + " ₽";
        }

        // ─────────────────────────────────────────────────────────────────────────
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            var supplier = SupplierCombo.SelectedItem as Suppliers;
            if (supplier == null)
                sb.AppendLine("Выберите поставщика.");
            if (_items.Count == 0)
                sb.AppendLine("Добавьте хотя бы одну позицию.");

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка");
                return;
            }

            using (var db = new AutoPartsStoreEntities())
            {
                var order = new PurchaseOrders
                {
                    SupplierID = supplier.SupplierID,
                    OrderDate = DateTime.Now,
                    TotalAmount = _items.Sum(i => i.Total),
                    Status = "Создан"
                };
                db.PurchaseOrders.Add(order);
                db.SaveChanges();               // получить PurchaseOrderID

                foreach (var it in _items)
                {
                    db.PurchaseOrderDetails.Add(new PurchaseOrderDetails
                    {
                        PurchaseOrderID = order.PurchaseOrderID,
                        PartID = it.PartID,
                        Quantity = it.Quantity,
                        UnitPrice = it.UnitPrice
                    });
                }

                db.SaveChanges();
            }

            MessageBox.Show("Заказ успешно создан!");
            GoHome();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => GoHome();

        private void GoHome()
        {
            if (_currentUser.RoleID == 1)
                _mw.NavigateTo(new AdminPage(_mw, _currentUser));
            else
                _mw.NavigateTo(new ManagerPage(_mw, _currentUser));
        }

        // ─────────────────────────────────────────────────────────────────────────
        private class OrderItem
        {
            public int PartID { get; set; }
            public string PartName { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Total { get { return UnitPrice * Quantity; } }
        }
    }
}
