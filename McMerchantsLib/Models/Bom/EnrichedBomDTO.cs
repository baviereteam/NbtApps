namespace McMerchantsLib.Models.Bom;

/// <summary>
/// A list of BOM items hydrated with a Searchable and optionally an inventory search result
/// </summary>
public class EnrichedBomDTO
{
    public ICollection<EnrichedBomItem> Items { get; set; } = new List<EnrichedBomItem>();
    public bool IsComplete { get; internal set; }
}
