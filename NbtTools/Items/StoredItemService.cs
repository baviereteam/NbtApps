using NbtTools.Geography;
using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Items
{
    public class StoredItemService : NbtService
    {
        private readonly string[] StorageIds;

        public StoredItemService(RegionQueryService regionQuery) : base(regionQuery)
        {
            StorageIds = new string[]
            {
                StorageType.BARREL.GetId(),
                StorageType.CHEST.GetId(),
                StorageType.TRAPPED_CHEST.GetId(),
                StorageType.SHULKERBOX.GetId()
            };
        }

        /// <summary>
        /// Indicates whether an item ID designates a shulker box, regardless of its color.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsShulkerBox(string id)
        {
            return id.Contains("shulker_box");
        }

        public IDictionary<Point, int> FindStoredItems(string searchedItem, Cuboid zone)
        {
            var dataSource = regionQuery.GetBlockEntitiesDataSource(zone, false);
            IDictionary<Point, int> results = new Dictionary<Point, int>();

            foreach (var entry in dataSource)
            {
                var blockEntity = entry.Tag;
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

                if (results.ContainsKey(position))
                {
                    results[position] += CountItemsIn(blockEntity, searchedItem);
                }
                else
                {
                    results[position] = CountItemsIn(blockEntity, searchedItem);
                }
            }
            
            return results;
        }

        public ICollection<string> ListStoredItems(Cuboid zone)
        {
            var dataSource = regionQuery.GetBlockEntitiesDataSource(zone, false);
            List<string> results = new List<string>();

            foreach (var entry in dataSource)
            {
                var blockEntity = entry.Tag;
                var containerIdTag = blockEntity["id"] as StringTag;
                if (!StorageIds.Contains(containerIdTag.Value))
                {
                    continue;
                }

                results.AddRange(ListItemsIn(blockEntity));
            }

            return results;
        }

        private ICollection<string> ListItemsIn(CompoundTag storage)
        {
            List<string> results = new List<string>();

            var itemsTag = storage["Items"] as ListTag;
            if (itemsTag == null)
            {
                return results;
            }

            // each non-empty slot in the container
            foreach (Tag t in itemsTag)
            {
                var itemTag = t as CompoundTag;
                var itemIdTag = itemTag["id"] as StringTag;

                if (IsShulkerBox(itemIdTag))
                {
                    results.AddRange(ListItemsInShulkerBox(itemTag));
                }
                else
                {
                    results.Add(itemIdTag.Value);
                }
            }

            return results;
        }

        private int CountItemsIn(CompoundTag storage, string searchedItem)
        {
            var itemsTag = storage["Items"] as ListTag;
            if (itemsTag == null)
            {
                return 0;
            }

            int count = 0;

            // each non-empty slot in the container
            foreach (Tag t in itemsTag)
            {
                var itemTag = t as CompoundTag;
                var itemIdTag = itemTag["id"] as StringTag;

                if (itemIdTag.Value == searchedItem)
                {
                    count += (itemTag["Count"] as ByteTag).Value;
                }

                else if (IsShulkerBox(itemIdTag))
                {
                    count += CountItemsInShulkerBox(itemTag, searchedItem);
                }
            }

            return count;
        }

        private ICollection<string> ListItemsInShulkerBox(CompoundTag shulkerBox)
        {
            List<string> results = new List<string>();

            var tagTag = shulkerBox["tag"] as CompoundTag;
            if (tagTag == null)
            {
                return results;
            }

            var blockEntityTag = tagTag["BlockEntityTag"] as CompoundTag;
            if (blockEntityTag == null)
            {
                return results;
            }

            return ListItemsIn(blockEntityTag);
        }

        private int CountItemsInShulkerBox(CompoundTag shulkerBox, string searchedItem)
        {
            var tagTag = shulkerBox["tag"] as CompoundTag;
            if (tagTag == null)
            {
                return 0;
            }

            var blockEntityTag = tagTag["BlockEntityTag"] as CompoundTag;
            if (blockEntityTag == null)
            {
                return 0;
            }

            return CountItemsIn(blockEntityTag, searchedItem);
        }
    }
}