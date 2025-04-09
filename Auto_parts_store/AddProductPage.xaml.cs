using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class AddProductPage : Page
    {
        private AutoParts _currentProduct;

        // Конструктор без параметров — для добавления
        public AddProductPage() : this(null) { }

        // Конструктор для редактирования
        public AddProductPage(AutoParts selectedProduct)
        {
            InitializeComponent();

            // Загружаем данные для ComboBox-ов
            CategoryComboBox.ItemsSource = Entities.GetContext().Categories.ToList();
            ModelComboBox.ItemsSource = Entities.GetContext().CarModels.ToList();

            if (selectedProduct != null)
            {
                // Получаем "чистый" объект из текущего контекста, чтобы избежать конфликтов
                _currentProduct = Entities.GetContext().AutoParts
                    .FirstOrDefault(p => p.PartID == selectedProduct.PartID);
            }
            else
            {
                _currentProduct = new AutoParts();
            }

            // Заполняем поля, если редактируем
            if (_currentProduct != null && _currentProduct.PartID != 0)
            {
                NameTextBox.Text = _currentProduct.PartName;
                PriceTextBox.Text = _currentProduct.Price.ToString();
                QuantityTextBox.Text = _currentProduct.StockQuantity.ToString();
                DescriptionTextBox.Text = _currentProduct.Description;
                CategoryComboBox.SelectedValue = _currentProduct.CategoryID;
                ModelComboBox.SelectedValue = _currentProduct.CarModelID;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var db = Entities.GetContext();
            StringBuilder errors = new StringBuilder();

            // Валидация
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                errors.AppendLine("Введите название.");
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                errors.AppendLine("Введите корректную цену.");
            if (!int.TryParse(QuantityTextBox.Text, out int quantity))
                errors.AppendLine("Введите корректное количество.");
            if (CategoryComboBox.SelectedItem == null)
                errors.AppendLine("Выберите категорию.");
            if (ModelComboBox.SelectedItem == null)
                errors.AppendLine("Выберите модель авто.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            // Заполняем объект
            _currentProduct.PartName = NameTextBox.Text;
            _currentProduct.Price = price;
            _currentProduct.StockQuantity = quantity;
            _currentProduct.Description = DescriptionTextBox.Text;
            _currentProduct.CategoryID = (int)CategoryComboBox.SelectedValue;
            _currentProduct.CarModelID = (int)ModelComboBox.SelectedValue;

            try
            {
                if (_currentProduct.PartID == 0)
                {
                    // Новый — добавить
                    db.AutoParts.Add(_currentProduct);
                }

                db.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");

                // Возврат назад
                if (NavigationService != null)
                    NavigationService.GoBack();
                else
                    Window.GetWindow(this)?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
    }
}
