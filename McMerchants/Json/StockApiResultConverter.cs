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
            WriteStoresDictionary(writer, value.Stores, options);

            writer.WritePropertyName("factories");
            WriteFactoriesDictionary(writer, value.Factories, options);

            writer.WriteEndObject();
        }

        private void WriteStoresDictionary(Utf8JsonWriter writer, IList<StoreStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (StoreStockResult storeResult in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", storeResult.Store.Name);
                writer.WriteString("logo", storeResult.Store.Logo == "" ? null : storeResult.Store.Logo);

                writer.WritePropertyName("alleys");
                writer.WriteStartArray();

                // Default alley
                if (storeResult.StockInDefaultAlley != null)
                {
                    WriteAlley(writer, true, storeResult.StockInDefaultAlley.Item1.Name, storeResult.StockInDefaultAlley.Item2);
                }

                // Other alleys
                foreach (KeyValuePair<Alley, int> alleyResult in storeResult.StockInOtherAlleys)
                {
                    WriteAlley(writer, false, alleyResult.Key.Name, alleyResult.Value);
                }

                // Bulk
                foreach (KeyValuePair<Point, int> bulkResult in storeResult.StockInBulkContainers) 
                {
                    WriteBulk(writer, bulkResult.Key, bulkResult.Value);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private void WriteAlley(Utf8JsonWriter writer, bool isDefault, string name, int count) {
            writer.WriteStartObject();
            if (isDefault) {
                writer.WriteString("type", "default");
            }
            writer.WriteString("name", name);
            writer.WriteNumber("count", count);
            writer.WriteEndObject();
        }

        private void WriteBulk(Utf8JsonWriter writer, Point point, int count) {
            writer.WriteStartObject();
            writer.WriteString("type", "bulk");
            writer.WriteNumber("x", point.X);
            writer.WriteNumber("y", point.Y);
            writer.WriteNumber("z", point.Z);
            writer.WriteNumber("count", count);
            writer.WriteEndObject();
        }

        private void WriteFactoriesDictionary(Utf8JsonWriter writer, IDictionary<FactoryRegion, IDictionary<Point, int>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<FactoryRegion, IDictionary<Point, int>> storeResult in value)
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
