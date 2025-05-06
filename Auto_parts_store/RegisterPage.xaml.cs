using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Auto_parts_store
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        private MainWindow _mainWindow;

        public RegisterPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = usernameTextBox.Text.Trim();
                string password = passwordBox.Password.Trim();
                string confirmPassword = confirmPasswordBox.Password.Trim();
                string fullName = fullNameTextBox.Text.Trim();
                string phone = phoneTextBox.Text.Trim();
                string email = emailTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Не все поля паролей заполнены.");
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают.");
                    return;
                }

                if (!Regex.IsMatch(email, @"^[\w\.\-]+@[\w\-]+\.[a-z]{2,4}$"))
                {
                    MessageBox.Show("Введите корректный email.");
                    return;
                }

                if (!Regex.IsMatch(phone, @"^(\+7|8)\d{10}$"))
                {
                    MessageBox.Show("Телефон должен быть в формате: +79991234567 или 89991234567.");
                    return;
                }

                using (var db = new AutoPartsStoreEntities())
                {
                    if (db.Users.Any(u => u.Username == login))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                        return;
                    }

                    var clientRole = db.Roles.FirstOrDefault(r => r.RoleName == "Client");
                    if (clientRole == null)
                    {
                        MessageBox.Show("Роль 'Client' не найдена в базе данных. Обратитесь к администратору.");
                        return;
                    }

                    var newUser = new Users
                    {
                        Username = login,
                        PasswordHash = HashHelper.GetHash(password),
                        FullName = fullName,
                        Phone = phone,
                        Email = email,
                        RoleID = clientRole.RoleID
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Регистрация прошла успешно!");
                    _mainWindow.NavigateTo(new LoginPage(_mainWindow));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка регистрации:\n" + ex.Message);
            }
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
        private void Phone_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9+]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new LoginPage(_mainWindow));
        }
    }
}
