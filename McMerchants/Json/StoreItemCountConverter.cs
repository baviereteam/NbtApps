using McMerchants.Models.Database;
using NbtTools.Geography;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McMerchants.Json
{
    public class StoreItemCountConverter : JsonConverter<IDictionary<StorageRegion, IDictionary<Point, int>>>
    {
        public override IDictionary<StorageRegion, IDictionary<Point, int>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IDictionary<StorageRegion, IDictionary<Point, int>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<StorageRegion, IDictionary<Point, int>> storeResult in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", storeResult.Key.Name);
                writer.WriteString("logo", storeResult.Key.Logo);

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
