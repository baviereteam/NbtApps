using NbtTools.Geography;
using NbtTools.Items.Providers;
using NbtTools.RegionQuery;
using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Items
{
    public class StoredItemService
    {
        private readonly StorageReaderFactory StorageReaderFactory;
        private readonly BlockEntitiesQuery RegionQuery;
        private readonly string[] StorageIds;
        private readonly string[] BookStorageIds;

        public StoredItemService(BlockEntitiesQuery regionQuery, StorageReaderFactory storageReaderFactory)
        {
            StorageIds = new string[]
            {
                StorageType.BARREL.GetId(),
                StorageType.CHEST.GetId(),
                StorageType.TRAPPED_CHEST.GetId(),
                StorageType.SHULKERBOX.GetId()
            };
            BookStorageIds = new string[]
            {
                StorageType.BARREL.GetId(),
                StorageType.CHEST.GetId(),
                StorageType.TRAPPED_CHEST.GetId(),
                StorageType.SHULKERBOX.GetId(),
                StorageType.CHISELED_BOOKSHELF.GetId()
            };

            StorageReaderFactory = storageReaderFactory;
            RegionQuery = regionQuery;
        }

        public CuboidItemsSearchResult FindStoredItems(ICollection<Searchable> searchedItems, Cuboid zone)
        {
            var results = new CuboidItemsSearchResult();
            bool searchContainsBooks = searchedItems.Any(item => item is EnchantedBook);
            var dataSource = RegionQuery.GetData(zone);
            results.UnreadableChunks = dataSource.UnreadableChunks;

            foreach (var versionedBlockEntity in dataSource.Result)
            {
                var container = versionedBlockEntity.Tag;
                var idTag = container["id"] as StringTag;

                // Check this blockentity is a container for the kind of items we search
                if (searchContainsBooks)
                {
                    if (!BookStorageIds.Contains(idTag.Value))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!StorageIds.Contains(idTag.Value))
                    {
                        continue;
                    }
                }

                Point position = new Point(
                    (container["x"] as IntTag).Value,
                    (container["y"] as IntTag).Value,
                    (container["z"] as IntTag).Value
                );

                var storageReader = StorageReaderFactory.GetForVersion(versionedBlockEntity.DataVersion);
                var containerContentsMatching = storageReader.CountMatchingItemsInContainer(container, searchedItems);

                foreach(var entry in containerContentsMatching)
                {
                    results.Add(position, entry.Key, entry.Value);
                }
            }

            return results;
        }
    }
}