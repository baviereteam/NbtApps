using McMerchantsLib.Models.Bom;
using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using McMerchantsLib.Models.Database;
using StockAtPosition = System.Collections.Generic.KeyValuePair<NbtTools.Geography.Point, int>;

namespace McMerchants.Json.Bom
{
    public class BomItemConverter : JsonConverter<EnrichedBomDTO>
    {
        public override EnrichedBomDTO Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, EnrichedBomDTO dto, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("complete", dto.IsComplete);

            writer.WritePropertyName("items");
            WriteItems(writer, dto.Items);

            writer.WriteEndObject();
        }

        private static void WriteItems(Utf8JsonWriter writer, ICollection<EnrichedBomItem> bomEntries)
        {
            writer.WriteStartArray();

            foreach (var bomEntry in bomEntries) 
            {
                writer.WriteStartObject();

                writer.WriteString("itemName", bomEntry.Item.Name);
                writer.WriteNumber("stackSize", bomEntry.Item.StackSize);
                writer.WriteNumber("requiredQuantity", bomEntry.RequiredQuantity);

                writer.WritePropertyName("availability");
                WriteAvailability(writer, bomEntry);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private static void WriteAvailability(Utf8JsonWriter writer, EnrichedBomItem item)
        {
            writer.WriteStartObject();

            if (item.WorkzoneQuantity.HasValue)
            {
                writer.WriteNumber("workzone", item.WorkzoneQuantity.Value);
            }
            else
            {
                writer.WriteNull("workzone");
            }

            if (item.StoredQuantities == null)
            {
	            var keys = new[] { "stores", "factories", "traders" };
	            foreach (var key in keys)
	            {
		            writer.WritePropertyName(key);
		            writer.WriteStartArray();
		            writer.WriteEndArray();
	            }
            }
            else
            {
	            writer.WritePropertyName("stores");
                WriteStoresDictionary(writer, item.StoredQuantities.Stores);

	            writer.WritePropertyName("factories");
                WriteFactoriesDictionary(writer, item.StoredQuantities.Factories);

	            writer.WritePropertyName("traders");
                WriteTradingDictionary(writer, item.StoredQuantities.Trades);
            }
            
            writer.WriteEndObject();
        }

        private static void WriteStoresDictionary(Utf8JsonWriter writer, ICollection<StoreItemStockResult> value)
        {
            writer.WriteStartArray();
            List<string> alleyNames = new List<string>();

            foreach (StoreItemStockResult storeResult in value)
            {
                int count = 0;

                // Default alley
                if (storeResult.StockInDefaultAlley != null)
                {
                    count += storeResult.StockInDefaultAlley.Item2;
                    alleyNames.Add(storeResult.StockInDefaultAlley.Item1.Name);
                }

                // Other alleys
                foreach (KeyValuePair<Alley, int> alleyResult in storeResult.StockInOtherAlleys)
                {
                    count += alleyResult.Value;
                    alleyNames.Add(alleyResult.Key.Name);
                }

                // Bulk
                if (storeResult.StockInBulkContainers.Count > 0)
                {
                    alleyNames.Add("bulk");

                    foreach (var bulkStock in storeResult.StockInBulkContainers)
                    {
                        count += bulkStock.Value;
                    }
                }

                if (count > 0)
                {
                    writer.WriteStartObject();
                    writer.WriteString("name", storeResult.Store.Name);
                    writer.WriteNumber("count", count);

                    writer.WritePropertyName("alleys");
                    writer.WriteStartArray();
                    foreach(var alley in alleyNames)
                    {
                        writer.WriteStringValue(alley);
                    }
                    writer.WriteEndArray();

                    writer.WriteEndObject();
                }
            }

            writer.WriteEndArray();
        }

        private static void WriteFactoriesDictionary(Utf8JsonWriter writer, ICollection<FactoryItemStockResult> value)
        {
            writer.WriteStartArray();

            foreach (var factoryResult in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", factoryResult.Factory.Name);

                int count = 0;
                foreach (StockAtPosition stackResult in factoryResult.Stock)
                {
                    count += stackResult.Value;
                }

                writer.WriteNumber("count", count);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private static void WriteTradingDictionary(Utf8JsonWriter writer, ICollection<TradeItemStockResult> value)
        {
            writer.WriteStartArray();

            foreach (var tradingPlace in value)
            {
                writer.WriteStringValue(tradingPlace.TradingPlace.Name);
            }

            writer.WriteEndArray();
        }
    }
}
