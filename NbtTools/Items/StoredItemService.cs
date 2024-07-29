using NbtTools.Geography;
using NbtTools.Items.Providers;
using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Items
{
    public class StoredItemService : NbtService
    {
        private readonly StorageReaderFactory StorageReaderFactory;
        private readonly string[] StorageIds;

        public StoredItemService(RegionQueryService regionQuery, StorageReaderFactory storageReaderFactory) : base(regionQuery)
        {
            StorageIds = new string[]
            {
                StorageType.BARREL.GetId(),
                StorageType.CHEST.GetId(),
                StorageType.TRAPPED_CHEST.GetId(),
                StorageType.SHULKERBOX.GetId()
            };

            StorageReaderFactory = storageReaderFactory;
        }

        public IDictionary<Point, int> FindStoredItems(string searchedItem, Cuboid zone)
        {
            var dataSource = regionQuery.GetBlockEntitiesDataSource(zone, false);
            IDictionary<Point, int> results = new Dictionary<Point, int>();

            foreach (var versionedBlockEntity in dataSource)
            {
                var blockEntity = versionedBlockEntity.Tag;

                var containerIdTag = blockEntity["id"] as StringTag;
                if (!StorageIds.Contains(containerIdTag.Value))
                {
                    continue;
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
            
            return results;
        }

        public ICollection<string> ListStoredItems(Cuboid zone)
        {
            var dataSource = regionQuery.GetBlockEntitiesDataSource(zone, false);
            List<string> results = new List<string>();

            foreach (var versionedBlockEntity in dataSource)
            {
                var blockEntity = versionedBlockEntity.Tag;
                var containerIdTag = blockEntity["id"] as StringTag;
                if (!StorageIds.Contains(containerIdTag.Value))
                {
                    continue;
                }

                var storageReader = StorageReaderFactory.GetForVersion(versionedBlockEntity.DataVersion);
                results.AddRange(storageReader.ListItemsIn(blockEntity));
            }

            return results;
        }
    }
}