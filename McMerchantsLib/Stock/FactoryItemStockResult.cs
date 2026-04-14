using McMerchants.Models.Database;
using NbtTools.Geography;

namespace McMerchantsLib.Stock
{
    /// <summary>
    /// Search results for a single item type in a factory.
    /// </summary>
    public class FactoryItemStockResult
    {
        public FactoryRegion Factory { get; set; }

        public IDictionary<Point, int> Stock { get; set; }
    }
}
