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
     * @brief A client for handling authentication-related API calls.
     */
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        /**
         * @brief Initializes a new instance of the AuthApiClient class.
         * @param baseUrl The base URL of the API.
         */
        public AuthApiClient(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler);
            _baseUrl = baseUrl.TrimEnd('/');
        }

        /**
         * @brief Sets the authorization token for the HTTP client.
         * @param token The authorization token.
         */
        public void SetToken(string token)
        {
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
            Console.WriteLine($"Token set: {token}");
        }

        /**
         * @brief Registers a new user.
         * @param registerRequest The registration details.
         * @return A task that represents the asynchronous operation. The task result contains the response string.
         * @exception Exception Thrown when the registration fails.
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
         * @brief Logs in an existing user.
         * @param loginRequest The login details.
         * @return A task that represents the asynchronous operation. The task result contains the response string.
         * @exception Exception Thrown when the login fails.
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

        /**
         * @brief Retrieves all projects.
         * @return A task that represents the asynchronous operation. The task result contains a collection of projects.
         * @exception Exception Thrown when the retrieval of projects fails.
         */
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Projects");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var projects = JsonSerializer.Deserialize<IEnumerable<Project>>(jsonResponse);
                return projects;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to load projects. Error: {error}");
            }
        }
    }
}
