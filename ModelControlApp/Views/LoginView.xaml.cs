using ModelControlApp.Services;
using ModelControlApp.ViewModels;
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
using System.Windows.Shapes;

namespace ModelControlApp.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            var authenticationService = new AuthenticationService();
            var viewModel = new LoginViewModel(authenticationService);
            viewModel.RequestClose += Close;
            DataContext = viewModel;            
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var viewModel = DataContext as LoginViewModel;
            if (viewModel != null)
            {
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
