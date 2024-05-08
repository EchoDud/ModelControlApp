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

namespace ModelControlApp.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly IAuthenticationService _authenticationService;

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            RegisterCommand = new DelegateCommand(async () => await RegisterAsync());
        }

        private async Task RegisterAsync()
        {

            /*var success = await _authenticationService.LoginAsync(Username, Password);*/
            var success = true;
            if (success)
                MessageBox.Show("Register Successful!");
            else
                MessageBox.Show("Register Failed!");
        }
    }
}
