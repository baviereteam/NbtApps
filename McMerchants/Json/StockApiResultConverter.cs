using McMerchants.Models.Database;
using McMerchants.Models.Json;
using NbtTools.Geography;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McMerchants.Json
{
    public class StockApiResultConverter : JsonConverter<StockApiResult>
    {
        public override StockApiResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, StockApiResult value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            writer.WritePropertyName("stores");
            WriteDictionary(writer, value.Stores, options);

            writer.WritePropertyName("factories");
            WriteDictionary(writer, value.Factories, options);

            writer.WriteEndObject();
        }

        private void WriteDictionary(Utf8JsonWriter writer, IDictionary<ItemProviderRegion, IDictionary<Point, int>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<ItemProviderRegion, IDictionary<Point, int>> storeResult in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", storeResult.Key.Name);
                writer.WriteString("logo", storeResult.Key.Logo == "" ? null : storeResult.Key.Logo);

                writer.WritePropertyName("results");
                writer.WriteStartArray();

                foreach (KeyValuePair<Point, int> stackResult in storeResult.Value)
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("x", stackResult.Key.X);
                    writer.WriteNumber("y", stackResult.Key.Y);
                    writer.WriteNumber("z", stackResult.Key.Z);
                    writer.WriteNumber("count", stackResult.Value);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

    }
}
