using ModelControlApp.ApiClients;
using ModelControlApp.DTOs.AuthDTOs;
using ModelControlApp.Infrastructure;
using ModelControlApp.ViewModels;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

public class LoginViewModel : AuthViewModelBase
{
    private readonly AuthApiClient _authApiClient;
    public event Action<string> RequestClose;
    public ICommand LoginCommand { get; }

    public LoginViewModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
        LoginCommand = new DelegateCommand(LoginUser);
    }

    private async void LoginUser()
    {
        IsBusy = true;
        try
        {
            var loginDto = new LoginDTO
            {
                Login = Username,
                Password = Password
            };

            var jsonResponse = await _authApiClient.LoginAsync(loginDto);
            var token = JsonPreprocessor.ExtractToken(jsonResponse);

            MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke(token);
        }
        catch (Exception ex)
        {
            string errorMessage = JsonPreprocessor.ExtractErrorMessage(ex.Message);
            NotifyError($"Login failed: {errorMessage}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
