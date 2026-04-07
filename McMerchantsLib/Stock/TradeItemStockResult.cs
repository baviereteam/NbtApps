using McMerchants.Models.Database;
using NbtTools.Entities.Trading;

namespace McMerchantsLib.Stock
{
    /// <summary>
    /// Search results for a single item type in a trading zone.
    /// </summary>
    public class TradeItemStockResult
    {
        public TradingRegion TradingPlace { get; set; }

        public ICollection<Trade> Trades { get; set; }
    }
}
