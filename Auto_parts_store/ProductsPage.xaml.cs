using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;



namespace Auto_parts_store
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {

        private MainWindow _mainWindow;
        private Users _currentUser;

        public ProductsPage(MainWindow mainWindow, Users currentUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentUser = currentUser;
            LoadParts();

            PartsDataGrid.ItemsSource = Entities.GetContext().AutoParts.ToList();
        }


        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                PartsDataGrid.ItemsSource = Entities.GetContext().AutoParts
                    .Include(p => p.Categories)
                    .Include(p => p.CarModels)
                    .ToList();
            }
        }

        private void LoadParts()
        {
            using (var db = new AutoPartsStoreEntities())
            {
                PartsDataGrid.ItemsSource = db.AutoParts
                    .Include(p => p.Categories)
                    .Include(p => p.CarModels)
                    .ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new AddProductPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = PartsDataGrid.SelectedItem as AutoParts;

            if (selectedPart == null)
            {
                MessageBox.Show("Пожалуйста, выберите запчасть для редактирования.");
                return;
            }

            _mainWindow.NavigateTo(new AddProductPage(selectedPart));  
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.RoleID == 1)
            {
                _mainWindow.NavigateTo(new AdminPage(_mainWindow, _currentUser));
            }
            else if (_currentUser.RoleID == 2)
            {
                _mainWindow.NavigateTo(new ManagerPage(_mainWindow, _currentUser));
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = PartsDataGrid.SelectedItems.Cast<AutoParts>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите элемент(ы) для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить {selectedItems.Count} элемент(ов)?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().AutoParts.RemoveRange(selectedItems);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Удаление выполнено.");
                    LoadParts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }

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

            PartIdSearchBox.Clear();
            LoadParts();
        }
    }
}
