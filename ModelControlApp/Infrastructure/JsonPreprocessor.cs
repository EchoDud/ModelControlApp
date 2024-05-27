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
     * @brief Static class for preprocessing JSON strings.
     */
    public static class JsonPreprocessor
    {
        /**
         * @brief Preprocesses a JSON string.
         * @param json The JSON string.
         * @return The preprocessed JSON string.
         */
        public static string PreprocessJson(string json)
        {
            json = Regex.Replace(json, @"ObjectId\(""(.+?)""\)", @"""$1""");
            json = Regex.Replace(json, @"ISODate\(""(.+?)""\)", @"""$1""");
            json = Regex.Replace(json, @"NumberLong\((\d+)\)", @"$1");
            return json;
        }

        /**
         * @brief Extracts a token from a JSON response.
         * @param jsonResponse The JSON response.
         * @return The extracted token.
         */
        public static string ExtractToken(string jsonResponse)
        {
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            return jsonDocument.RootElement.GetProperty("token").GetString();
        }

        /**
         * @brief Extracts an error message from a JSON response.
         * @param jsonResponse The JSON response.
         * @return The extracted error message.
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
                return "An unknown error occurred.";
            }
        }
    }
}
