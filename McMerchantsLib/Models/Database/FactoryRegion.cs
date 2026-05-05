namespace McMerchants.Models.Database
{
    /// <summary>
    /// Represents a place (cuboid) with an item farm.
    /// </summary>
    public class FactoryRegion : ItemProviderRegion
    {
        public const string TYPE_KEY = "factory";
        public new string Type { get; } = TYPE_KEY;

        public ICollection<FactoryProduct> Products { get; set; }
    }
}
