using System.Collections.Generic;

namespace McMerchants.Models.Database
{
    /// <summary>
    /// Represents a place (cuboid) with storage containers which content can be consulted (a chests room, for example).
    /// </summary>
    public class StorageRegion : ItemProviderRegion
    {
        public const string TYPE_KEY = "storage";
        public new string Type { get; } = TYPE_KEY;

        public ICollection<Alley>? Alleys { get; set; }
    }
}
