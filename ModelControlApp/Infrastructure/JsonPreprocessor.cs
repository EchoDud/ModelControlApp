using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelControlApp.Infrastructure
{
    /**
     * @class JsonPreprocessor
     * @brief Статический класс для предобработки JSON-строк.
     */
    public static class JsonPreprocessor
    {
        /**
         * @brief Предобрабатывает JSON-строку.
         * @param json JSON-строка.
         * @return Предобработанная JSON-строка.
         */
        public static string PreprocessJson(string json)
        {
            json = Regex.Replace(json, @"ObjectId\(""(.+?)""\)", @"""$1""");
            json = Regex.Replace(json, @"ISODate\(""(.+?)""\)", @"""$1""");
            json = Regex.Replace(json, @"NumberLong\((\d+)\)", @"$1");
            return json;
        }

        /**
         * @brief Извлекает токен из JSON-ответа.
         * @param jsonResponse JSON-ответ.
         * @return Извлеченный токен.
         */
        public static string ExtractToken(string jsonResponse)
        {
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            return jsonDocument.RootElement.GetProperty("token").GetString();
        }

        /**
         * @brief Извлекает сообщение об ошибке из JSON-ответа.
         * @param jsonResponse JSON-ответ.
         * @return Извлеченное сообщение об ошибке.
         */
        public static string ExtractErrorMessage(string jsonResponse)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                return jsonDocument.RootElement.GetProperty("title").GetString();
            }
            catch
            {
                return "Произошла неизвестная ошибка.";
            }
        }
    }
}
