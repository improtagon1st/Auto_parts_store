using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class CartPage : Page
    {
        private MainWindow _mainWindow;
        private Users _user;
        private List<AutoParts> _cart;

        public CartPage(MainWindow mainWindow, Users user, List<AutoParts> cart)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _user = user;
            _cart = cart;

            CartListView.ItemsSource = _cart;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new ClientPage(_mainWindow, _user, _cart));
        }
    }
}
