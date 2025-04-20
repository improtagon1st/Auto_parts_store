using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class SuppliersPage : Page
    {
        private MainWindow _mainWindow;
        private Users _user;

        public SuppliersPage(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _user = user;
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            SuppliersDataGrid.ItemsSource = Entities.GetContext().Suppliers.ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                LoadSuppliers();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new AddSupplierPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = SuppliersDataGrid.SelectedItem as Suppliers;
            if (selected == null)
            {
                MessageBox.Show("Выберите поставщика для редактирования.");
                return;
            }

            _mainWindow.NavigateTo(new AddSupplierPage(selected));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SuppliersDataGrid.SelectedItems.Cast<Suppliers>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите поставщика(ов) для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить {selectedItems.Count} поставщика(ов)?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Suppliers.RemoveRange(selectedItems);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Удаление выполнено.");
                    LoadSuppliers();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.RoleID == 1) // Админ
            {
                _mainWindow.NavigateTo(new AdminPage(_mainWindow, _user));
            }
            else if (_user.RoleID == 2) // Менеджер
            {
                _mainWindow.NavigateTo(new ManagerPage(_mainWindow, _user));
            }
        }
    }
}
