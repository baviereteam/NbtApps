using Humanizer;
using McMerchants.Models.Database;
using McMerchants.Services;
using McMerchantsLib.Stock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using NbtTools.Entities;
using NbtTools.Entities.Trading;
using System.Collections.Generic;
using System.Text.Json;

using StockAtPosition = System.Collections.Generic.KeyValuePair<NbtTools.Geography.Point, int>;

namespace McMerchants.Json
{
    public class WebApiConverter : StockApiResultConverter
    {
        private readonly ItemProviderLinksBuilder _mapLinkBuilder;

        public WebApiConverter(IStringLocalizer<Enchantment> enchantmentLocalizer, IStringLocalizer<Villager> villagerJobLocalizer, ItemProviderLinksBuilder mapLinkBuilder) : base(enchantmentLocalizer, villagerJobLocalizer)
        {
            _mapLinkBuilder = mapLinkBuilder;
        }

        protected override void WriteStoresDictionary(Utf8JsonWriter writer, ICollection<StoreItemStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (StoreItemStockResult storeResult in value)
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
                foreach (StockAtPosition bulkResult in storeResult.StockInBulkContainers)
                {
                    WriteBulk(writer, bulkResult.Key, bulkResult.Value);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        protected override void WriteFactoriesDictionary(Utf8JsonWriter writer, ICollection<FactoryItemStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var factoryResult in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("identity");
                WriteIdentity(writer, factoryResult.Factory);

                writer.WritePropertyName("results");
                writer.WriteStartArray();

                foreach (StockAtPosition stackResult in factoryResult.Stock)
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

        protected override void WriteTradingDictionary(Utf8JsonWriter writer, ICollection<TradeItemStockResult> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var tradingPlaceResult in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("identity");
                WriteIdentity(writer, tradingPlaceResult.TradingPlace, true);

                writer.WritePropertyName("results");
                writer.WriteStartArray();

                foreach (Trade trade in tradingPlaceResult.Trades)
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

            if (_mapLinkBuilder.CanGenerateMapLinks)
            {
                writer.WriteString("map_url", _mapLinkBuilder.GetMapUrlFor(place));
            }
            else
            {
                writer.WriteNull("map_url");
            }

            writer.WriteEndObject();
        }
    }
}
