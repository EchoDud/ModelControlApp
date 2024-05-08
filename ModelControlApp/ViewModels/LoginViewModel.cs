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

namespace ModelControlApp.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly IAuthenticationService _authenticationService;
        private string _username;
        private string _password;

        public event Action RequestClose;

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            LoginCommand = new DelegateCommand(async () => await LoginAsync());
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

        private async Task LoginAsync()
        {

            var success = await _authenticationService.LoginAsync(_username, _password);
            if (success)
            {
                MessageBox.Show("Login Successful!");
                RequestClose?.Invoke();
            }                
            else
            {
                MessageBox.Show("Login Failed!");
            }
            
                
        }

        
    }
}
