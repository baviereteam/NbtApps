using Humanizer;
using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using NbtTools.Entities;
using NbtTools.Entities.Trading;
using NbtTools.Geography;
using System.Collections.Generic;
using System.Text.Json;

namespace McMerchants.Json
{
    public class WebApiConverter : StockApiResultConverter
    {
        private readonly bool generateMapLinks = false;
        private readonly string webMapPattern = null;
        
        public WebApiConverter(IStringLocalizer<Enchantment> enchantmentLocalizer, IStringLocalizer<Villager> villagerJobLocalizer, IConfiguration configuration) : base(enchantmentLocalizer, villagerJobLocalizer)
        {
            var pattern = configuration["Options:WebmapUrlPattern"];
            generateMapLinks = !string.IsNullOrWhiteSpace(pattern);

            if (generateMapLinks)
            {
                // Must match the order used in GetWebMapUrl()
                webMapPattern = pattern
                    .Replace("{DIMENSION}", "{0}")
                    .Replace("{X}", "{1}")
                    .Replace("{Y}", "{2}")
                    .Replace("{Z}", "{3}");
            }
        }

        protected override void WriteStoresDictionary(Utf8JsonWriter writer, IList<StoreStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (StoreStockResult storeResult in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("identity");
                WriteIdentity(writer, storeResult.Store);

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

        protected override void WriteFactoriesDictionary(Utf8JsonWriter writer, IDictionary<FactoryRegion, IDictionary<Point, int>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<FactoryRegion, IDictionary<Point, int>> factoryResult in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("identity");
                WriteIdentity(writer, factoryResult.Key);

                writer.WritePropertyName("results");
                writer.WriteStartArray();

                foreach (KeyValuePair<Point, int> stackResult in factoryResult.Value)
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

        protected override void WriteTradingDictionary(Utf8JsonWriter writer, IDictionary<TradingRegion, IEnumerable<Trade>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<TradingRegion, IEnumerable<Trade>> tradingPlace in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("identity");
                WriteIdentity(writer, tradingPlace.Key, true);

                writer.WritePropertyName("results");
                writer.WriteStartArray();

                foreach (Trade trade in tradingPlace.Value)
                {
                    writer.WriteStartObject();

                    WriteVillager(writer, trade.Villager);
                    WriteTradeComponent(writer, "buy1", trade.Buy1);
                    WriteTradeComponent(writer, "buy2", trade.Buy2);
                    WriteTradeComponent(writer, "sell", trade.Sell);

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private void WriteIdentity(Utf8JsonWriter writer, ItemProviderRegion place, bool includeId = false)
        {
            writer.WriteStartObject();

            if (includeId)
            {
                writer.WriteNumber("id", place.Id);
            }            

            writer.WriteString("name", place.Name);
            writer.WriteString("logo", string.IsNullOrWhiteSpace(place.Logo) ? null : place.Logo);
            writer.WriteString("url", string.IsNullOrWhiteSpace(place.URL) ? null : place.URL);

            if (generateMapLinks)
            {
                // Must match the order set in the constructor
                writer.WriteString(
                    "map_url", 
                    webMapPattern.FormatWith(
                        place.Dimension,
                        place.StartX + (place.EndX - place.StartX) / 2,
                        place.StartY + (place.EndY - place.StartY) / 2,
                        place.StartZ + (place.EndZ - place.StartZ) / 2
                    )
                );
            }
            else
            {
                writer.WriteNull("map_url");
            }

            writer.WriteEndObject();
        }
    }
}
