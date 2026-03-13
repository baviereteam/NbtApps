using NbtTools.Geography;
using NbtTools.Items.Providers;
using NbtTools.RegionQuery;
using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;

using StoredItem = System.Collections.Generic.KeyValuePair<NbtTools.Geography.Point, int>;

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

        public QueryResult<StoredItem> FindStoredItems(Searchable searchedItem, Cuboid zone)
        {
            var dataSource = RegionQuery.GetData(zone);
            IDictionary<Point, int> results = new Dictionary<Point, int>();

            foreach (var versionedBlockEntity in dataSource.Result)
            {
                var blockEntity = versionedBlockEntity.Tag;

                var containerIdTag = blockEntity["id"] as StringTag;

                switch (searchedItem)
                {
                    case EnchantedBook _:
                        // Enchanted books can be in chiseled bookshelves
                        if (!BookStorageIds.Contains(containerIdTag.Value))
                        {
                            continue;
                        }
                        break;

                    default:
                        if (!StorageIds.Contains(containerIdTag.Value))
                        {
                            continue;
                        }
                        break;
                }

                Point position = new Point(
                    (blockEntity["x"] as IntTag).Value,
                    (blockEntity["y"] as IntTag).Value,
                    (blockEntity["z"] as IntTag).Value
                );

                var storageReader = StorageReaderFactory.GetForVersion(versionedBlockEntity.DataVersion);

                if (results.ContainsKey(position))
                {
                    results[position] += storageReader.CountItemsIn(versionedBlockEntity.Tag, searchedItem);
                }
                else
                {
                    results[position] = storageReader.CountItemsIn(versionedBlockEntity.Tag, searchedItem);
                }
            }
            
            return new QueryResult<StoredItem>(results, dataSource.UnreadableChunks);
        }
    }
}