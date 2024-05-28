using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.ApiClients
{
    /**
     * @interface IBaseApiClient
     * @brief Интерфейс для базового API-клиента.
     */
    public interface IBaseApiClient
    {
        /**
         * @brief Устанавливает токен аутентификации.
         * @param token Токен.
         */
        void SetToken(string token);
    }
}
