using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class OrderHistoryPage : Page
    {
        private MainWindow _mainWindow;
        private Users _currentUser;
        private List<AutoParts> _cart;

        public OrderHistoryPage(MainWindow mainWindow, Users currentUser, List<AutoParts> cart)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = currentUser;
            _cart = cart;

            LoadOrderHistory();
        }

        private void LoadOrderHistory()
        {
            var db = Entities.GetContext();

            var orders = db.Sales
                .Include(s => s.SalesDetails.Select(d => d.AutoParts))
                .Where(s => s.UserID == _currentUser.UserID)
                .OrderByDescending(s => s.SaleDate)
                .ToList();

            OrdersListView.ItemsSource = orders;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new ClientPage(_mainWindow, _currentUser, _cart));

        }

    }
}
