using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string hashedPassword = HashHelper.GetHash(password);

            try
            {
                using (var db = new AutoPartsStoreEntities()) 
                {
                    var user = db.Users.Include("Roles")
                                       .FirstOrDefault(u => u.Username == username && u.PasswordHash == hashedPassword);

                    if (user != null)
                    {

                        switch (user.Roles.RoleName)
                        {
                            case "Administrator":
                                _mainWindow.NavigateTo(new AdminPage(_mainWindow, user));
                                break;
                            case "Manager":
                                _mainWindow.NavigateTo(new ManagerPage(_mainWindow, user));
                                break;
                            case "Cashier":
                                _mainWindow.NavigateTo(new CashierPage(_mainWindow, user));
                                break;
                            case "Client":
                                _mainWindow.NavigateTo(new ClientPage(_mainWindow, user, new List<AutoParts>()));
                                break;
                            default:
                                MessageBox.Show("Неизвестная роль пользователя");
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}");
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
