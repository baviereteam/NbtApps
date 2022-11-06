using System.Collections.Generic;

namespace NbtTools.Entities.Trading
{
    public class TradeComponent
    {
        public string Item { get; private set; }
        public byte Quantity { get; private set; }
        public ICollection<string> Metadata { get; private set; }

        public TradeComponent(string item, byte quantity, ICollection<string> metadata)
        {
            Item = item;
            Quantity = quantity;
            Metadata = metadata;
        }

        public override string ToString()
        {
            return $"{Quantity} {Item}" + (Metadata.Count > 0 ? " (with metadata)" : "");
        }
    }

    public class Trade
    {
        public TradeComponent Buy1 { get; private set; }
        public TradeComponent Buy2 { get; private set; }
        public TradeComponent Sell { get; private set; }

        public Trade(TradeComponent buy1, TradeComponent buy2, TradeComponent sell)
        {
            Buy1 = buy1;
            Buy2 = buy2;
            Sell = sell;
        }

        public override string ToString()
        {
            return $"{Buy1} + {Buy2} -> {Sell}";
        }
    }
}
