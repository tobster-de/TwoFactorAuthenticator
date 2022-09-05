using System.Windows;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var viewModel = this.ViewModel;
            viewModel.Issuer = "Example Org";
            viewModel.Account = "test@account.com";
            viewModel.Secret = "f68f1fe894d548a1bbc66165c46e61eb";
        }

        public MainViewModel ViewModel => (MainViewModel)DataContext;
    }
}