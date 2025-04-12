using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System.Collections.Generic;

namespace Auto_parts_store
{
    public partial class OrderHistoryPage : Page
    {
        private MainWindow _mainWindow;
        private Users _currentUser;

        public OrderHistoryPage(MainWindow mainWindow, Users currentUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = currentUser;

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
            new ClientPage(_mainWindow, _currentUser, new List<AutoParts>());

        }
    }
}
