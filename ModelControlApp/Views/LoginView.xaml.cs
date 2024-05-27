/**
 * @file LoginView.xaml.cs
 * @brief Interaction logic for LoginView.xaml
 */

using ModelControlApp.ApiClients;
using ModelControlApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ModelControlApp.Views
{
    /**
     * @class LoginView
     * @brief Interaction logic for LoginView.xaml
     */
    public partial class LoginView : Window
    {
        /**
         * @brief Initializes a new instance of the LoginView class.
         */
        public LoginView()
        {
            InitializeComponent();
            var authClient = new AuthApiClient("http://localhost:5000/");
            var viewModel = new LoginViewModel(authClient);
            viewModel.RequestClose += (token) =>
            {
                ((LocalVersionControlViewModel)Application.Current.MainWindow.DataContext).AuthToken = token;
                ((LocalVersionControlViewModel)Application.Current.MainWindow.DataContext).IsLoggedIn = true;
                Close();
            };
            DataContext = viewModel;
        }

        /**
         * @brief Handles the PasswordChanged event of the PasswordBox control.
         * @param sender The source of the event.
         * @param e The RoutedEventArgs instance containing the event data.
         */
        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
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
