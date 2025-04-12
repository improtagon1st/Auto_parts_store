using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class ClientPage : Page
    {
        private MainWindow _mainWindow;
        private Users _user;
        private List<AutoParts> _cart;

        // ✅ Перегрузка конструктора без корзины
        public ClientPage(MainWindow mainWindow, Users user)
            : this(mainWindow, user, new List<AutoParts>())
        {
        }

        public ClientPage(MainWindow mainWindow, Users user, List<AutoParts> cart)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _user = user;
            _cart = cart ?? new List<AutoParts>();

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
            if (sender is Button button && button.Tag is AutoParts part)
            {
                _cart.Add(part);
                MessageBox.Show($"Товар \"{part.PartName}\" добавлен в корзину.");
            }
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new CartPage(_mainWindow, _user, _cart));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new LoginPage(_mainWindow));
        }
    }
}
