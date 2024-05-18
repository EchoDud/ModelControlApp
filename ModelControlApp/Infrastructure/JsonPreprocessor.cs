using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelControlApp.Infrastructure
{
    public static class JsonPreprocessor
    {
        public static string PreprocessJson(string json)
        {
            // Replace ObjectId("...") with "..."
            json = Regex.Replace(json, @"ObjectId\(""(.+?)""\)", @"""$1""");

            // Replace ISODate("...") with "..."
            json = Regex.Replace(json, @"ISODate\(""(.+?)""\)", @"""$1""");

            // Replace NumberLong(...) with ...
            json = Regex.Replace(json, @"NumberLong\((\d+)\)", @"$1");

            return json;
        }
    }
}
