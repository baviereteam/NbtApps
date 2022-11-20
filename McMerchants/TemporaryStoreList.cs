using NbtTools.Geography;
using System.Collections;
using System.Collections.Generic;

namespace McMerchants
{
    public class TemporaryStoreList
    {
        private static readonly IDictionary<string, Cuboid> stores = new Dictionary<string, Cuboid>()
        {
            { "Marvin's Hardware Store", new Cuboid(new Point(-226, 77, -2164), new Point(-263, 72, -2132)) },
            { "Li'l Blossoms Plant Store", new Cuboid(new Point(-264, 64, -2202), new Point(-253, 67, -2218)) },
        };
        
        public static IDictionary<string, Cuboid> GetStores()
        {
            return stores;
        }
    }
}
