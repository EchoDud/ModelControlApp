using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelControlApp.Infrastructure
{
    public static class JsonPreprocessor
    {
        public static string PreprocessJson(string json)
        {
            json = Regex.Replace(json, @"ObjectId\(""(.+?)""\)", @"""$1""");
            json = Regex.Replace(json, @"ISODate\(""(.+?)""\)", @"""$1""");
            json = Regex.Replace(json, @"NumberLong\((\d+)\)", @"$1");
            return json;
        }

        public static string ExtractToken(string jsonResponse)
        {
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            return jsonDocument.RootElement.GetProperty("token").GetString();
        }

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
