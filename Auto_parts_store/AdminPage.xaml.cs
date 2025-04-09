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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private MainWindow _mainWindow;
        private Users _user;

        public AdminPage(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _user = user;
            LoadUsers();
            LoadAutoParts();
            LoadSuppliers();
            LoadOrders();
            LoadInventory();
        }

        private void LoadUsers()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                UsersDataGrid.ItemsSource = db.Users.Include("Roles").ToList();
            }
        }

        private void LoadAutoParts()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                AutoPartsDataGrid.ItemsSource = db.AutoParts.ToList();
            }
        }

        private void LoadSuppliers()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                SuppliersDataGrid.ItemsSource = db.Suppliers.ToList();
            }
        }

        private void LoadOrders()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                OrdersDataGrid.ItemsSource = db.PurchaseOrders.ToList();
            }
        }

        private void LoadInventory()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                InventoryDataGrid.ItemsSource = db.Inventory.ToList();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new LoginPage(_mainWindow));
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
           // _mainWindow.NavigateTo(new AddEmployeePage(_mainWindow));
        }
    }
}