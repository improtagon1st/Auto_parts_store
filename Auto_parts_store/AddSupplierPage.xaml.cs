using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class AddSupplierPage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly Users _currentUser;
        private readonly Suppliers _currentSupplier;

        // Конструктор «добавить»
        public AddSupplierPage(MainWindow mw, Users user)
            : this(mw, user, null) { }

        // Конструктор «редактировать»
        public AddSupplierPage(MainWindow mw, Users user, Suppliers selectedSupplier)
        {
            InitializeComponent();

            _mainWindow = mw;
            _currentUser = user;

            // либо свежая копия выбранного, либо новый объект
            _currentSupplier = selectedSupplier != null
                ? Entities.GetContext().Suppliers
                           .FirstOrDefault(s => s.SupplierID == selectedSupplier.SupplierID)
                : new Suppliers();

            // Привязываем объект к XAML‑разметке
            DataContext = _currentSupplier;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var db = Entities.GetContext();
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentSupplier.SupplierName))
                errors.AppendLine("Введите название.");
            if (string.IsNullOrWhiteSpace(_currentSupplier.ContactName))
                errors.AppendLine("Введите контактное лицо.");
            if (string.IsNullOrWhiteSpace(_currentSupplier.Phone))
                errors.AppendLine("Введите телефон.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            try
            {
                if (_currentSupplier.SupplierID == 0)
                    db.Suppliers.Add(_currentSupplier);   // это новый

                db.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");

                // Возврат на страницу‑список
                if (_currentUser.RoleID == 1)       // админ
                    _mainWindow.NavigateTo(new AdminPage(_mainWindow, _currentUser));
                else                                // менеджер
                    _mainWindow.NavigateTo(new ManagerPage(_mainWindow, _currentUser));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
    }
}
