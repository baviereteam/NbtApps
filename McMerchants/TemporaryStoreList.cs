using McMerchants.Models;
using NbtTools.Geography;
using System.Collections;
using System.Collections.Generic;

namespace McMerchants
{
    public class TemporaryStoreList
    {
        private static readonly ICollection<Store> stores = new List<Store>()
        {
            new Store("Marvin's Hardware Store", "marvins.png", new Point(-226, 77, -2164), new Point(-263, 72, -2132)),
            new Store("Li'l Blossoms Plant Store", "lilblossoms.png", new Point(-264, 64, -2202), new Point(-253, 67, -2218))
        };
        
        public static ICollection<Store> GetStores()
        {
            return stores;
        }
    }
}
