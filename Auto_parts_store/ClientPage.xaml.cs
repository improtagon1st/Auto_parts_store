using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class ClientPage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly Users _user;
        private readonly List<AutoParts> _cart;

        public ClientPage(MainWindow mainWindow, Users user)
            : this(mainWindow, user, new List<AutoParts>()) { }

        public ClientPage(MainWindow mainWindow, Users user, List<AutoParts> cart)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _user = user;
            _cart = cart ?? new List<AutoParts>();

            LoadFilters();
            ApplyFilters();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool becameVisible = (bool)e.NewValue;          
            if (!becameVisible) return;

            var ctx = Entities.GetContext();
            foreach (var entry in ctx.ChangeTracker.Entries())
                entry.Reload();

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

            var cat = CategoryComboBox.SelectedItem as Categories;
            var mdl = ModelComboBox.SelectedItem as CarModels;

            if (cat != null)
                parts = parts.Where(p => p.CategoryID == cat.CategoryID);

            if (mdl != null)
                parts = parts.Where(p => p.CarModelID == mdl.CarModelID);

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                parts = parts.Where(p => p.PartName.Contains(SearchTextBox.Text));

            PartsListView.ItemsSource = parts.ToList();
        }

        private void Filter_Changed(object sender, RoutedEventArgs e) => ApplyFilters();

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var part = btn != null ? btn.Tag as AutoParts : null;  

            if (part != null)
            {
                _cart.Add(part);
                MessageBox.Show($"Товар «{part.PartName}» добавлен в корзину.");
            }
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e) =>
            _mainWindow.NavigateTo(new CartPage(_mainWindow, _user, _cart));

        private void ViewOrderHistory_Click(object sender, RoutedEventArgs e) =>
            _mainWindow.NavigateTo(new OrderHistoryPage(_mainWindow, _user, _cart));

        private void BackButton_Click(object sender, RoutedEventArgs e) =>
            _mainWindow.NavigateTo(new LoginPage(_mainWindow));
    }
}
