namespace NbtTools.Entities.Trading
{

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
