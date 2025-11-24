using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.Localization;
using NbtTools.Entities;
using NbtTools.Entities.Trading;
using NbtTools.Geography;
using System.Collections.Generic;
using System.Text.Json;

namespace McMerchants.Json
{
    public class PluginApiConverter : StockApiResultConverter
    {
        public PluginApiConverter(IStringLocalizer<Enchantment> enchantmentLocalizer, IStringLocalizer<Villager> villagerJobLocalizer) : base(enchantmentLocalizer, villagerJobLocalizer)
        {
        }

        protected override void WriteStoresDictionary(Utf8JsonWriter writer, IList<StoreStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (StoreStockResult storeResult in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", storeResult.Store.Name);

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
                writer.WriteEndArray();

                // Bulk
                int itemCountInBulk = 0;
                foreach (KeyValuePair<Point, int> bulkResult in storeResult.StockInBulkContainers)
                {
                    itemCountInBulk += bulkResult.Value;
                }
                writer.WriteNumber("bulk", itemCountInBulk);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        protected override void WriteFactoriesDictionary(Utf8JsonWriter writer, IDictionary<FactoryRegion, IDictionary<Point, int>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<FactoryRegion, IDictionary<Point, int>> storeResult in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", storeResult.Key.Name);

                int countItemsInFactory = 0;
                foreach (KeyValuePair<Point, int> stackResult in storeResult.Value)
                {
                    countItemsInFactory += stackResult.Value;
                }
                writer.WriteNumber("count", countItemsInFactory);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        protected override void WriteTradingDictionary(Utf8JsonWriter writer, IDictionary<TradingRegion, IEnumerable<Trade>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<TradingRegion, IEnumerable<Trade>> tradingPlace in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", tradingPlace.Key.Name);

                int countTradesInZone = 0;
                foreach (Trade trade in tradingPlace.Value)
                {
                    countTradesInZone++;
                }
                writer.WriteNumber("count", countTradesInZone);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
