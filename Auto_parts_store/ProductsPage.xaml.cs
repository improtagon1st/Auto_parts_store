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
using System.Data.Entity;
using Auto_parts_store;



namespace Auto_parts_store
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            LoadParts();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());
                LoadParts();
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
            NavigationService.Navigate(new AddProductPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = PartsDataGrid.SelectedItem as AutoParts;
            if (selectedPart == null)
            {
                MessageBox.Show("Выберите элемент для редактирования.");
                return;
            }

            NavigationService.Navigate(new AddProductPage(selectedPart));
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
    }
}
