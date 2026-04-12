using McMerchantsLib.Models.Database;
using McMerchantsLib.Stock;
using NbtTools.Items;

namespace McMerchantsLib.Models.Bom;

public class EnrichedBomItem
{
	private BomItem _bomItem;
	
	public EnrichedBomItem(BomItem bomItem, Searchable item)
	{
		_bomItem = bomItem;
		Item = item;
	}
	
	public int RequiredQuantity => _bomItem.RequiredQuantity;
	
    public Searchable Item { get; set; }

    // Availability
    public int? WorkzoneQuantity { get; set; } = null;
    public ItemStockResult? StoredQuantities { get; set; } = null;
}