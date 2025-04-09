using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            LoadParts();
            UpdateCartDisplay();
        }

        private void LoadParts()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                PartsDataGrid.ItemsSource = db.AutoParts.Where(p => p.StockQuantity > 0).ToList();
            }
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
                    Quantity = quantity
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
            public decimal Total => Price * Quantity;
        }
    }
}
