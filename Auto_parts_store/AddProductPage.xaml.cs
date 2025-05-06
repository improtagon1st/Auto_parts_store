using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class AddProductPage : Page
    {
        /* --- новые поля --- */
        private readonly MainWindow _mainWindow;
        private readonly Users _currentUser;

        private readonly AutoParts _currentProduct;

        /* ---------- КОНСТРУКТОРЫ ---------- */

        // для добавления (MainWindow / user приходят с вызывающей страницы)
        public AddProductPage(MainWindow mw, Users user) : this(mw, user, null) { }

        // для редактирования
        public AddProductPage(MainWindow mw, Users user, AutoParts selectedProduct)
        {
            InitializeComponent();

            _mainWindow = mw;
            _currentUser = user;

            CategoryComboBox.ItemsSource = Entities.GetContext().Categories.ToList();
            ModelComboBox.ItemsSource = Entities.GetContext().CarModels.ToList();

            _currentProduct = selectedProduct != null
                ? Entities.GetContext().AutoParts.First(p => p.PartID == selectedProduct.PartID)
                : new AutoParts();

            DataContext = _currentProduct;          // ← привязка
        }

        /* ---------- выбор картинки (осталось как было) ---------- */

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Файлы изображений|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (dlg.ShowDialog() == true)
            {
                var projDir = AppDomain.CurrentDomain.BaseDirectory;
                var imgDir = Path.Combine(projDir, "Images", "Parts");
                Directory.CreateDirectory(imgDir);

                var destFile = Path.Combine(imgDir, Path.GetFileName(dlg.FileName));
                if (!File.Exists(destFile))
                    File.Copy(dlg.FileName, destFile);

                ImagePathTextBox.Text = $"/Images/Parts/{Path.GetFileName(destFile)}";
            }
        }

        /* ---------- СОХРАНЕНИЕ ---------- */

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProduct.PartName))
                sb.AppendLine("Введите название.");
            if (_currentProduct.Price <= 0)
                sb.AppendLine("Введите корректную цену.");
            if (_currentProduct.StockQuantity < 0)
                sb.AppendLine("Введите корректное количество.");
            if (_currentProduct.CategoryID == 0)
                sb.AppendLine("Выберите категорию.");
            if (_currentProduct.CarModelID == 0)
                sb.AppendLine("Выберите модель авто.");
            if (string.IsNullOrWhiteSpace(_currentProduct.ImagePath))
                sb.AppendLine("Укажите путь к изображению.");

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка");
                return;
            }

            if (_currentProduct.PartID == 0)
                Entities.GetContext().AutoParts.Add(_currentProduct);

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");

                /* ---------- НАВИГАЦИЯ ПОСЛЕ СОХРАНЕНИЯ ---------- */
                if (_mainWindow != null && _currentUser != null)
                {
                    if (_currentUser.RoleID == 1)          // администратор
                        _mainWindow.NavigateTo(new AdminPage(_mainWindow, _currentUser));
                    else if (_currentUser.RoleID == 2)     // менеджер
                        _mainWindow.NavigateTo(new ManagerPage(_mainWindow, _currentUser));
                    else                                   // другие роли
                        _mainWindow.NavigateTo(new LoginPage(_mainWindow));
                }
                else
                {
                    // fallback
                    NavigationService?.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
    }
}
