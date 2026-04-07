using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.Localization;
using NbtTools.Entities;
using NbtTools.Entities.Trading;
using NbtTools.Geography;
using System.Collections.Generic;
using System.Text.Json;

using StockAtPosition = System.Collections.Generic.KeyValuePair<NbtTools.Geography.Point, int>;

namespace McMerchants.Json
{
    public class PluginApiConverter : StockApiResultConverter
    {
        public PluginApiConverter(IStringLocalizer<Enchantment> enchantmentLocalizer, IStringLocalizer<Villager> villagerJobLocalizer) : base(enchantmentLocalizer, villagerJobLocalizer)
        {
        }

        protected override void WriteStoresDictionary(Utf8JsonWriter writer, ICollection<StoreItemStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var storeEntry in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", storeEntry.Store.Name);

                writer.WritePropertyName("alleys");
                writer.WriteStartArray();

                // Default alley
                if (storeEntry.StockInDefaultAlley != null)
                {
                    WriteAlley(writer, true, storeEntry.StockInDefaultAlley.Item1.Name, storeEntry.StockInDefaultAlley.Item2);
                }

                // Other alleys
                foreach (KeyValuePair<Alley, int> alleyResult in storeEntry.StockInOtherAlleys)
                {
                    WriteAlley(writer, false, alleyResult.Key.Name, alleyResult.Value);
                }
                writer.WriteEndArray();

                // Bulk
                int itemCountInBulk = 0;
                foreach (KeyValuePair<Point, int> bulkResult in storeEntry.StockInBulkContainers)
                {
                    itemCountInBulk += bulkResult.Value;
                }
                writer.WriteNumber("bulk", itemCountInBulk);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        protected override void WriteFactoriesDictionary(Utf8JsonWriter writer, ICollection<FactoryItemStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var factoryEntry in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", factoryEntry.Factory.Name);

                int countItemsInFactory = 0;
                foreach (StockAtPosition stackResult in factoryEntry.Stock)
                {
                    countItemsInFactory += stackResult.Value;
                }
                writer.WriteNumber("count", countItemsInFactory);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        protected override void WriteTradingDictionary(Utf8JsonWriter writer, ICollection<TradeItemStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var tradingPlaceEntry in value)
            {
                writer.WriteStartObject();
                writer.WriteString("name", tradingPlaceEntry.TradingPlace.Name);

                int countTradesInZone = 0;
                foreach (Trade trade in tradingPlaceEntry.Trades)
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
