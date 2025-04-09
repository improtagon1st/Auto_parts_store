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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private MainWindow _mainWindow;
        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = usernameTextBox.Text.Trim();
            string password = passwordBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) ||
                login == usernameTextBox.Tag.ToString() || password == passwordBox.Tag.ToString())
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            string hashed = HashHelper.GetHash(password);

            using (var db = new AutoPartsStoreEntities())
            {
                var user = db.Users.Include("Roles").FirstOrDefault(u =>
                    u.Username == login && u.PasswordHash == hashed);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль.");
                    return;
                }

                switch (user.Roles.RoleName)
                {
                    case "Admin":
                        
                        break;
                    case "Manager":
                        
                        break;
                    case "Cashier":
                       
                        break;
                    case "Client":
                        
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль.");
                        break;
                }
            }
        }

        private void RegisterLabel_Click(object sender, MouseButtonEventArgs e)
        {
            _mainWindow.NavigateTo(new RegisterPage(_mainWindow));

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.Text == tb.Tag.ToString())
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = tb.Tag.ToString();
                tb.Foreground = Brushes.Gray;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb.Password == pb.Tag.ToString())
            {
                pb.Password = "";
                pb.Foreground = Brushes.Black;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (string.IsNullOrWhiteSpace(pb.Password))
            {
                pb.Password = pb.Tag.ToString();
                pb.Foreground = Brushes.Gray;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb.Password != pb.Tag.ToString())
            {
                pb.Foreground = Brushes.Black;
            }
        }
    }
}
