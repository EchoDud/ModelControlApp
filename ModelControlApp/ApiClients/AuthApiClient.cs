using ModelControlApp.DTOs.AuthDTOs;
using ModelControlApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModelControlApp.ApiClients
{
    /**
     * @class AuthApiClient
     * @brief Клиент для выполнения операций аутентификации.
     */
    public class AuthApiClient : BaseApiClient
    {
        /**
         * @brief Конструктор с базовым URL.
         * @param baseUrl Базовый URL.
         */
        public AuthApiClient(string baseUrl) : base(baseUrl) { }

        /**
         * @brief Выполняет регистрацию пользователя.
         * @param registerRequest Данные для регистрации.
         * @return Результат регистрации.
         * @exception Exception Вызывается в случае ошибки.
         */
        public async Task<string> RegisterAsync(RegisterDTO registerRequest)
        {
            var registerContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");
            var registerResponse = await _httpClient.PostAsync($"{_baseUrl}/api/Auth/register", registerContent);

            if (registerResponse.IsSuccessStatusCode)
            {
                var registerResult = await registerResponse.Content.ReadAsStringAsync();
                return registerResult;
            }
            else
            {
                var error = await registerResponse.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        /**
         * @brief Выполняет вход пользователя.
         * @param loginRequest Данные для входа.
         * @return Результат входа.
         * @exception Exception Вызывается в случае ошибки.
         */
        public async Task<string> LoginAsync(LoginDTO loginRequest)
        {
            var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await _httpClient.PostAsync($"{_baseUrl}/api/Auth/login", loginContent);

            if (loginResponse.IsSuccessStatusCode)
            {
                var loginResult = await loginResponse.Content.ReadAsStringAsync();
                return loginResult;
            }
            else
            {
                var error = await loginResponse.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }
    }
}
