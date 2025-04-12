using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class CartPage : Page
    {
        private MainWindow _mainWindow;
        private Users _user;
        private List<AutoParts> _cart;

        public CartPage(MainWindow mainWindow, Users user, List<AutoParts> cart)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _user = user;
            _cart = cart;

            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            var displayItems = _cart.Select(p => new
            {
                p.PartID,
                p.PartName,
                p.Price,
                Quantity = 1,
                Total = p.Price
            }).ToList();

            CartDataGrid.ItemsSource = displayItems;
            TotalTextBlock.Text = displayItems.Sum(i => i.Total).ToString("0.00") + " ₽";
        }

        private void CompleteOrderButton_Click(object sender, RoutedEventArgs e)
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
                    TotalAmount = _cart.Sum(i => i.Price),
                    UserID = _user.UserID
                };
                db.Sales.Add(sale);
                db.SaveChanges();

                foreach (var part in _cart)
                {
                    db.SalesDetails.Add(new SalesDetails
                    {
                        SaleID = sale.SaleID,
                        PartID = part.PartID,
                        Quantity = 1,
                        UnitPrice = part.Price
                    });

                    var partInDb = db.AutoParts.FirstOrDefault(p => p.PartID == part.PartID);
                    if (partInDb != null)
                        partInDb.StockQuantity -= 1;
                }

                db.SaveChanges();
            }

            MessageBox.Show("Заказ оформлен!");
            _cart.Clear();
            _mainWindow.NavigateTo(new ClientPage(_mainWindow, _user, _cart));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new ClientPage(_mainWindow, _user, _cart));
        }
    }
}

