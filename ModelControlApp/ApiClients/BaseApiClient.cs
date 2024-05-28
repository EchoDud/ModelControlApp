using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.ApiClients
{
    /**
     * @class BaseApiClient
     * @brief Базовый клиент для выполнения API-запросов.
     */
    public abstract class BaseApiClient : IBaseApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;

        /**
         * @brief Конструктор с базовым URL.
         * @param baseUrl Базовый URL.
         */
        protected BaseApiClient(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler);
            _baseUrl = baseUrl.TrimEnd('/');
        }

        /**
         * @brief Устанавливает токен аутентификации.
         * @param token Токен.
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
        }
    }
}
