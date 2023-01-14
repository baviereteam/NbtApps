using System.Collections.Generic;

namespace McMerchants.Models.Database
{
    /// <summary>
    /// Represents a place (cuboid) with an item farm.
    /// </summary>
    public class FactoryRegion : ItemProviderRegion
    {
        public ICollection<FactoryProduct> Products { get; set; }
    }
}
