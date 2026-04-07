using McMerchantsLib.Stock;

namespace McMerchants.Models.DTO
{
    public class SingleItemStockResultDTO
    {
        public ItemStockResult SearchResult { get; set; }
        public bool IsComplete { get; set; }
    }
}
