using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.Localization;
using NbtTools.Entities;
using NbtTools.Entities.Trading;
using NbtTools.Geography;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McMerchants.Json
{
    public class StockApiResultConverter : JsonConverter<StockQueryResult>
    {
        private readonly IStringLocalizer<Enchantment> _enchantmentLocalizer;
        private readonly IStringLocalizer<Villager> _villagerJobLocalizer;

        public StockApiResultConverter(
            IStringLocalizer<Enchantment> enchantmentLocalizer,
            IStringLocalizer<Villager> villagerJobLocalizer
        ) 
        { 
            _enchantmentLocalizer = enchantmentLocalizer;
            _villagerJobLocalizer = villagerJobLocalizer;
        }

        public override StockQueryResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, StockQueryResult value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            writer.WritePropertyName("stores");
            WriteStoresDictionary(writer, value.Stores, options);

            writer.WritePropertyName("factories");
            WriteFactoriesDictionary(writer, value.Factories, options);

            writer.WritePropertyName("traders");
            WriteTradingDictionary(writer, value.Trades, options);

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

        private void WriteTradingDictionary(Utf8JsonWriter writer, IDictionary<TradingRegion, IEnumerable<Trade>> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<TradingRegion, IEnumerable<Trade>> tradingPlace in value)
            {
                writer.WriteStartObject();
                writer.WriteNumber("id", tradingPlace.Key.Id);
                writer.WriteString("name", tradingPlace.Key.Name);
                writer.WriteString("logo", tradingPlace.Key.Logo == "" ? null : tradingPlace.Key.Logo);

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

        private void WriteVillager(Utf8JsonWriter writer, Villager villager)
        {
            writer.WritePropertyName("villager");
            writer.WriteStartObject();
            writer.WriteString("job", _villagerJobLocalizer[villager.Job]);
            writer.WriteNumber("x", Math.Floor(villager.Position.X));
            writer.WriteNumber("y", Math.Floor(villager.Position.Y));
            writer.WriteNumber("z", Math.Floor(villager.Position.Z));
            writer.WriteEndObject();
        }

        private void WriteTradeComponent(Utf8JsonWriter writer, string propertyName, TradeComponent component)
        {
            writer.WritePropertyName(propertyName);

            if (component == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();
            writer.WriteString("item", component.Item.Name);
            writer.WriteString("id", component.Item.Id);
            writer.WriteNumber("quantity", component.Quantity);

            writer.WritePropertyName("enchantments");
            writer.WriteStartArray();
            foreach (var enchantment in component.Enchantments)
            {
                writer.WriteStartObject();
                writer.WriteString("name", _enchantmentLocalizer[enchantment.Id]);
                writer.WriteNumber("level", enchantment.Level);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
