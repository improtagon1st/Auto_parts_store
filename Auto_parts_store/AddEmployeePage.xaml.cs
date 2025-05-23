﻿    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    namespace Auto_parts_store
    {
        /// <summary>
        /// Логика взаимодействия для AddEmployeePage.xaml
        /// </summary>
        public partial class AddEmployeePage : Page
        {
            private MainWindow _mainWindow;
            private Users _user;


            public AddEmployeePage(MainWindow mainWindow, Users user)
            {
                InitializeComponent();
                _mainWindow = mainWindow;
                _user = user;
            }

            private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
            {
                string username = usernameTextBox.Text;
                string password = passwordBox.Password;
                string role = (roleComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                string fullName = fullNameTextBox.Text;
                string email = emailTextBox.Text;

                try
                {
                    using (var db = new AutoPartsStoreEntities())
                    {
                        var existingUser = db.Users.FirstOrDefault(u => u.Username == username);
                        if (existingUser != null)
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!");
                            return;
                        }

                        var newUser = new Users
                        {
                            Username = username,
                            PasswordHash = HashHelper.GetHash(password),
                            FullName = fullName,
                            Email = email,
                            RoleID = role == "Manager" ? 2 : (role == "Cashier" ? 3 : 4)
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges(); 

                        MessageBox.Show("Сотрудник успешно добавлен!");

                        usernameTextBox.Clear();
                        passwordBox.Clear();
                        fullNameTextBox.Clear();
                        emailTextBox.Clear();
                        roleComboBox.SelectedIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}");
                }
            }
            private void BackButton_Click(object sender, RoutedEventArgs e)
            {
                _mainWindow.NavigateTo(new AdminPage(_mainWindow, _user));
            }
        }
    }
