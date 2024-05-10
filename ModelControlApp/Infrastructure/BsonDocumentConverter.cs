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
    public class BsonDocumentConverter : JsonConverter<BsonDocument>
    {
        public override BsonDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonDocument.ParseValue(ref reader);
            return BsonDocument.Parse(json.RootElement.GetRawText());
        }

        public override void Write(Utf8JsonWriter writer, BsonDocument value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToJson());
        }
    }
}
