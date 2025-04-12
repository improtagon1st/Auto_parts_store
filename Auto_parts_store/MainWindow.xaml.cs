using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Auto_parts_store
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateTo(new LoginPage(this));

        }
        public void NavigateTo(Page page)
        {
            this.Content = page;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
