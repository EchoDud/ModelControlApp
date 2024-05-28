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
     * @brief JSON-конвертер для BsonDocument.
     */
    public class BsonDocumentConverter : JsonConverter<BsonDocument>
    {
        /**
         * @brief Читает BsonDocument из JSON.
         * @param reader JSON-ридер.
         * @param typeToConvert Тип для конвертации.
         * @param options Опции сериализации.
         * @return BsonDocument.
         */
        public override BsonDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonDocument.ParseValue(ref reader);
            return BsonDocument.Parse(json.RootElement.GetRawText());
        }

        /**
         * @brief Записывает BsonDocument в JSON.
         * @param writer JSON-райтер.
         * @param value Значение BsonDocument.
         * @param options Опции сериализации.
         */
        public override void Write(Utf8JsonWriter writer, BsonDocument value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToJson());
        }
    }
}
