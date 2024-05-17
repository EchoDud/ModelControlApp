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
using ModelControlApp.ApiClients;
using ModelControlApp.DTOs.AuthDTOs;

namespace ModelControlApp.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly AuthApiClient _authApiClient;
        private string _login;
        private string _email;
        private string _password;

        public event Action<string> RequestClose;

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(AuthApiClient authApiClient)
        {
            _authApiClient = authApiClient;
            RegisterCommand = new DelegateCommand(RegisterUser);
        }

        public string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private async void RegisterUser()
        {
            try
            {
                var registerDto = new RegisterDTO
                {
                    Login = Login,
                    Email = Email,
                    Password = Password
                };

                var token = await _authApiClient.RegisterAsync(registerDto);
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke(token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
