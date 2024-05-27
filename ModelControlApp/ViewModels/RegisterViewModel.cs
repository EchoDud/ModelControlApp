using ModelControlApp.ApiClients;
using ModelControlApp.DTOs.AuthDTOs;
using ModelControlApp.Infrastructure;
using ModelControlApp.ViewModels;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

public class RegisterViewModel : AuthViewModelBase
{
    private readonly AuthApiClient _authApiClient;
    private string _email;

    public event Action<string> RequestClose;
    public ICommand RegisterCommand { get; }

    public RegisterViewModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
        RegisterCommand = new DelegateCommand(RegisterUser);
    }

    public string Email
    {
        get { return _email; }
        set { SetProperty(ref _email, value); }
    }

    private async void RegisterUser()
    {
        IsBusy = true;
        try
        {
            var registerDto = new RegisterDTO
            {
                Login = Username,
                Email = Email,
                Password = Password
            };

            var jsonResponse = await _authApiClient.RegisterAsync(registerDto);
            var token = JsonPreprocessor.ExtractToken(jsonResponse);

            MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke(token);
        }
        catch (Exception ex)
        {
            string errorMessage = JsonPreprocessor.ExtractErrorMessage(ex.Message);
            NotifyError($"Registration failed: {errorMessage}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
