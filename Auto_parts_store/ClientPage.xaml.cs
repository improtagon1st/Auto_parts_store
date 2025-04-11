using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class ClientPage : Page
    {
        private MainWindow _mainWindow;
        private Users _user;

        public ClientPage(MainWindow mainWindow, Users user)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _user = user;

            LoadFilters();
            ApplyFilters();
        }

        private void LoadFilters()
        {
            CategoryComboBox.ItemsSource = Entities.GetContext().Categories.ToList();
            CategoryComboBox.DisplayMemberPath = "CategoryName";

            ModelComboBox.ItemsSource = Entities.GetContext().CarModels.ToList();
            ModelComboBox.DisplayMemberPath = "Model";
        }

        private void ApplyFilters()
        {
            var parts = Entities.GetContext().AutoParts.AsQueryable();

            if (CategoryComboBox.SelectedItem is Categories selectedCategory)
                parts = parts.Where(p => p.CategoryID == selectedCategory.CategoryID);

            if (ModelComboBox.SelectedItem is CarModels selectedModel)
                parts = parts.Where(p => p.CarModelID == selectedModel.CarModelID);

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                parts = parts.Where(p => p.PartName.Contains(SearchTextBox.Text));

            PartsListView.ItemsSource = parts.ToList();
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is AutoParts part)
            {
                MessageBox.Show($"Товар \"{part.PartName}\" добавлен в корзину.");
                // Тут можно добавить логику добавления в корзину
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new LoginPage(_mainWindow));
        }
    }
}
