using NbtTools.Geography;
using NbtTools.Items.Providers;
using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;

using StoredItems = System.Collections.Generic.IDictionary<NbtTools.Geography.Point, int>;

namespace NbtTools.Items
{
    public class StoredItemService : NbtService
    {
        private readonly StorageReaderFactory StorageReaderFactory;
        private readonly string[] StorageIds;
        private readonly string[] BookStorageIds;

        public StoredItemService(RegionQueryService regionQuery, StorageReaderFactory storageReaderFactory) : base(regionQuery)
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
        }

        public QueryResult<StoredItems> FindStoredItems(Searchable searchedItem, Cuboid zone)
        {
            var dataSource = regionQuery.GetBlockEntitiesDataSource(zone);
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
            
            return new QueryResult<StoredItems>(results, dataSource.UnreadableChunks);
        }
    }
}