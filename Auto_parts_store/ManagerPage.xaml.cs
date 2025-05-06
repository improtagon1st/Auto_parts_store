using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    /// <summary>
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {
        private Users _user;
        private MainWindow _mainWindow;

        public ManagerPage(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _user = user;

            LoadAutoParts();
            LoadSuppliers();
            LoadOrders();
            LoadOrderDetails();
            LoadSales();
            LoadSalesDetails();
            LoadCategories();
            LoadCarModels();
            LoadInventory();
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

        private void LoadOrderDetails()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                OrderDetailsDataGrid.ItemsSource = db.PurchaseOrderDetails.ToList();
            }
        }

        private void LoadSales()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                SalesDataGrid.ItemsSource = db.Sales.ToList();
            }
        }

        private void LoadSalesDetails()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                SalesDetailsDataGrid.ItemsSource = db.SalesDetails.ToList();
            }
        }

        private void LoadCategories()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                CategoriesDataGrid.ItemsSource = db.Categories.ToList();
            }
        }

        private void LoadCarModels()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                CarModelsDataGrid.ItemsSource = db.CarModels.ToList();
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

        private void GoToProductsPage_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new ProductsPage(_mainWindow, _user));
        }
        private void GoToSuppliersPage_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new SuppliersPage(_mainWindow, _user));
        }
        private void GoToReportsPage_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new ReportsPage(_mainWindow, _user));
        }
        private void OpenPurchaseOrderPage_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(
                new AddPurchaseOrderPage(_mainWindow, _user));
        }


    }
}
