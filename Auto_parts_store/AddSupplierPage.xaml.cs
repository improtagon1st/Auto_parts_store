using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Auto_parts_store
{
    public partial class AddSupplierPage : Page
    {

        private Suppliers _currentSupplier;

        public AddSupplierPage(Suppliers selectedSupplier)
        {
            InitializeComponent();

            _currentSupplier = selectedSupplier ?? new Suppliers();

            if (selectedSupplier != null)
            {
                NameTextBox.Text = _currentSupplier.SupplierName;
                ContactNameTextBox.Text = _currentSupplier.ContactName;
                PhoneTextBox.Text = _currentSupplier.Phone;
                EmailTextBox.Text = _currentSupplier.Email;
                AddressTextBox.Text = _currentSupplier.Address;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var db = Entities.GetContext();
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                errors.AppendLine("Введите название.");
            if (string.IsNullOrWhiteSpace(ContactNameTextBox.Text))
                errors.AppendLine("Введите контактное лицо.");
            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                errors.AppendLine("Введите телефон.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            _currentSupplier.SupplierName = NameTextBox.Text;
            _currentSupplier.ContactName = ContactNameTextBox.Text;
            _currentSupplier.Phone = PhoneTextBox.Text;
            _currentSupplier.Email = EmailTextBox.Text;
            _currentSupplier.Address = AddressTextBox.Text;

            try
            {
                if (_currentSupplier.SupplierID == 0)
                    db.Suppliers.Add(_currentSupplier);

                db.SaveChanges();
                MessageBox.Show("Сохранение прошло успешно!");
                NavigationService?.GoBack();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
       
    }
}
