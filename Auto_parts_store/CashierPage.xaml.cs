using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    /// <summary>
    /// Логика взаимодействия для CashierPage.xaml
    /// </summary>
    public partial class CashierPage : Page
    {
        private MainWindow _mainWindow;
        private Users _cashier;
        private List<CartItem> _cart = new List<CartItem>();

        public CashierPage(MainWindow mainWindow, Users cashier)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _cashier = cashier;

            LoadCarMakes();
            LoadParts();
            UpdateCartDisplay();
        }

        private void LoadCarMakes()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                CarMakeComboBox.ItemsSource = db.CarModels
                    .Select(c => c.Make)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();
            }
        }

        private void LoadParts()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                var parts = db.AutoParts
                    .Include(p => p.CarModels)
                    .Where(p => p.StockQuantity > 0)
                    .ToList();

                PartsDataGrid.ItemsSource = parts;
            }
        }

        private void ApplyFilters()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                var query = db.AutoParts
                    .Include(p => p.CarModels)
                    .Where(p => p.StockQuantity > 0);

                if (CarMakeComboBox.SelectedItem is string make)
                {
                    query = query.Where(p => p.CarModels.Make == make);
                }

                PartsDataGrid.ItemsSource = query.ToList();
            }
        }

        private void CarMakeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchByIdButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PartIdSearchBox.Text, out int partId))
            {
                using (var db = new AutoPartsStoreEntities())
                {
                    var part = db.AutoParts
                        .Include(p => p.CarModels)
                        .FirstOrDefault(p => p.PartID == partId);

                    if (part != null)
                    {
                        PartsDataGrid.ItemsSource = new List<AutoParts> { part };
                    }
                    else
                    {
                        MessageBox.Show("Запчасть с таким ID не найдена.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите корректный ID.");
            }
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            CarMakeComboBox.SelectedIndex = -1;
            PartIdSearchBox.Clear();
            LoadParts();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = PartsDataGrid.SelectedItem as AutoParts;
            if (selectedPart == null)
            {
                MessageBox.Show("Пожалуйста, выберите запчасть.");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            if (quantity > selectedPart.StockQuantity)
            {
                MessageBox.Show("Недостаточно товара на складе.");
                return;
            }

            var existing = _cart.FirstOrDefault(c => c.PartID == selectedPart.PartID);
            if (existing != null)
            {
                if (existing.Quantity + quantity > selectedPart.StockQuantity)
                {
                    MessageBox.Show("Превышено доступное количество.");
                    return;
                }
                existing.Quantity += quantity;
            }
            else
            {
                _cart.Add(new CartItem
                {
                    PartID = selectedPart.PartID,
                    PartName = selectedPart.PartName,
                    Price = selectedPart.Price,
                    Quantity = quantity,
                    Make = selectedPart.CarModels?.Make
                });
            }

            QuantityTextBox.Clear();
            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            CartDataGrid.ItemsSource = null;
            CartDataGrid.ItemsSource = _cart;
            TotalTextBlock.Text = _cart.Sum(i => i.Total).ToString("0.00") + " ₽";
        }

        private void CompleteSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста.");
                return;
            }

            using (var db = new AutoPartsStoreEntities())
            {
                var sale = new Sales
                {
                    SaleDate = DateTime.Now,
                    TotalAmount = _cart.Sum(c => c.Total),
                    UserID = _cashier.UserID
                };

                db.Sales.Add(sale);
                db.SaveChanges();

                foreach (var item in _cart)
                {
                    db.SalesDetails.Add(new SalesDetails
                    {
                        SaleID = sale.SaleID,
                        PartID = item.PartID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });

                    var part = db.AutoParts.FirstOrDefault(p => p.PartID == item.PartID);
                    if (part != null)
                    {
                        part.StockQuantity -= item.Quantity;
                    }
                }

                db.SaveChanges();
            }

            MessageBox.Show("Продажа успешно оформлена!");
            _cart.Clear();
            UpdateCartDisplay();
            LoadParts();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new LoginPage(_mainWindow));
        }

        private class CartItem
        {
            public int PartID { get; set; }
            public string PartName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string Make { get; set; }
            public decimal Total => Price * Quantity;
        }
    }
}
