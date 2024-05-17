﻿using ModelControlApp.ApiClients;
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
    /// Логика взаимодействия для RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
            var authClient = new AuthApiClient("http://localhost:5000/");
            var viewModel = new RegisterViewModel(authClient);
            viewModel.RequestClose += (token) =>
            {
                ((LocalVersionControlViewModel)Application.Current.MainWindow.DataContext).AuthToken = token;
                ((LocalVersionControlViewModel)Application.Current.MainWindow.DataContext).IsLoggedIn = true;
                Close();
            };
            DataContext = viewModel;
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var viewModel = DataContext as RegisterViewModel;
            if (viewModel != null)
            {
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
