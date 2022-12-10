using McMerchants.Models.Database;
using NbtTools.Geography;
using System.Collections;
using System.Collections.Generic;

namespace McMerchants
{
    public class TemporaryStoreList
    {
        private static readonly ICollection<StorageRegion> stores = new List<StorageRegion>()
        {
            /*new Store("Marvin's Hardware Store", "marvins.png", "overworld", new Point(-226, 77, -2164), new Point(-263, 72, -2132)),
            new Store("Li'l Blossoms Plant Store", "lilblossoms.png", "overworld", new Point(-264, 64, -2202), new Point(-253, 67, -2218)),
            new Store("Piglin & Piglin", "", "nether", new Point(138, 77, 70), new Point(134, 79, 47))*/
        };
        
        public static ICollection<StorageRegion> GetStores()
        {
            return stores;
        }
    }
}
