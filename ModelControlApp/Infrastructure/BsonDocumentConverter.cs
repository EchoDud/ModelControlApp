using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModelControlApp.Infrastructure
{
    /**
     * @class BsonDocumentConverter
     * @brief JSON converter for BsonDocument.
     */
    public class BsonDocumentConverter : JsonConverter<BsonDocument>
    {
        /**
         * @brief Reads a BsonDocument from JSON.
         * @param reader The JSON reader.
         * @param typeToConvert The type to convert.
         * @param options The serializer options.
         * @return The BsonDocument.
         */
        public override BsonDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonDocument.ParseValue(ref reader);
            return BsonDocument.Parse(json.RootElement.GetRawText());
        }

        /**
         * @brief Writes a BsonDocument to JSON.
         * @param writer The JSON writer.
         * @param value The BsonDocument value.
         * @param options The serializer options.
         */
        public override void Write(Utf8JsonWriter writer, BsonDocument value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToJson());
        }
    }
}
