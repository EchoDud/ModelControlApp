using ModelControlApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using ModelControlApp.ApiClients;
using ModelControlApp.DTOs.AuthDTOs;

namespace ModelControlApp.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly AuthApiClient _authApiClient;
        private string _username;
        private string _password;

        public event Action RequestClose;

        public ICommand LoginCommand { get; }

        public LoginViewModel(AuthApiClient authApiClient)
        {

            _authApiClient = authApiClient;
            LoginCommand = new DelegateCommand(LoginUser);
        }

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private async void LoginUser()
        {
            try
            {
                var loginDto = new LoginDTO
                {
                    LoginOrEmail = Username, // Replace with actual user input
                    Password = Password // Replace with actual user input
                };

                var token = await _authApiClient.LoginAsync(loginDto);
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
