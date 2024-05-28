using ModelControlApp.ApiClients;
using ModelControlApp.DTOs.AuthDTOs;
using ModelControlApp.Infrastructure;
using ModelControlApp.ViewModels;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

/**
 * @class LoginViewModel
 * @brief Модель представления для входа в систему.
 */
public class LoginViewModel : AuthViewModelBase
{
    private readonly AuthApiClient _authApiClient;
    public event Action<string> RequestClose;
    public ICommand LoginCommand { get; }

    /**
     * @brief Конструктор с параметрами для инициализации.
     * @param authApiClient Клиент для аутентификации.
     */
    public LoginViewModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
        LoginCommand = new DelegateCommand(LoginUser);
    }

    /**
     * @brief Метод для выполнения входа пользователя.
     */
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

            NotifyInfo("Login successful!");
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
