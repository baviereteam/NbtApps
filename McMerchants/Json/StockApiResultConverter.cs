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
    public abstract class StockApiResultConverter : JsonConverter<StockQueryResult>
    {
        protected readonly IStringLocalizer<Enchantment> _enchantmentLocalizer;
        protected readonly IStringLocalizer<Villager> _villagerJobLocalizer;

        protected StockApiResultConverter(
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

            writer.WriteBoolean("complete", value.IsComplete);

            writer.WritePropertyName("stores");
            WriteStoresDictionary(writer, value.Stores, options);

            writer.WritePropertyName("factories");
            WriteFactoriesDictionary(writer, value.Factories, options);

            writer.WritePropertyName("traders");
            WriteTradingDictionary(writer, value.Trades, options);

            writer.WriteEndObject();
        }

        protected abstract void WriteStoresDictionary(Utf8JsonWriter writer, IList<StoreStockResult> value, JsonSerializerOptions options);
        
        protected void WriteAlley(Utf8JsonWriter writer, bool isDefault, string name, int count) {
            writer.WriteStartObject();
            if (isDefault) {
                writer.WriteString("type", "default");
            }
            writer.WriteString("name", name);
            writer.WriteNumber("count", count);
            writer.WriteEndObject();
        }

        protected void WriteBulk(Utf8JsonWriter writer, Point point, int count) {
            writer.WriteStartObject();
            writer.WriteString("type", "bulk");
            writer.WriteNumber("x", point.X);
            writer.WriteNumber("y", point.Y);
            writer.WriteNumber("z", point.Z);
            writer.WriteNumber("count", count);
            writer.WriteEndObject();
        }

        protected abstract void WriteFactoriesDictionary(Utf8JsonWriter writer, IDictionary<FactoryRegion, IDictionary<Point, int>> value, JsonSerializerOptions options);

        protected abstract void WriteTradingDictionary(Utf8JsonWriter writer, IDictionary<TradingRegion, IEnumerable<Trade>> value, JsonSerializerOptions options);

        protected void WriteVillager(Utf8JsonWriter writer, Villager villager)
        {
            writer.WritePropertyName("villager");
            writer.WriteStartObject();
            writer.WriteString("job", _villagerJobLocalizer[villager.Job]);
            writer.WriteNumber("x", Math.Floor(villager.Position.X));
            writer.WriteNumber("y", Math.Floor(villager.Position.Y));
            writer.WriteNumber("z", Math.Floor(villager.Position.Z));
            writer.WriteEndObject();
        }

        protected void WriteTradeComponent(Utf8JsonWriter writer, string propertyName, TradeComponent component)
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
