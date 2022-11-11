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

        public StoredItemService()
        {
            StorageIds = new string[]
            {
                StorageType.BARREL.GetId(),
                StorageType.CHEST.GetId(),
                StorageType.SHULKERBOX.GetId()
            };
        }

        public IDictionary<Point, int> FindStoredItems(string searchedItem, Cuboid zone)
        {
            var dataSource = regionQuery.GetBlockEntitiesDataSource(zone, false);
            IDictionary<Point, int> results = new Dictionary<Point, int>();

            foreach (var blockEntity in dataSource)
            {
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

                var itemsTag = blockEntity["Items"] as ListTag;

                foreach(Tag t in itemsTag)
                {
                    var itemTag = t as CompoundTag;
                    var itemIdTag = itemTag["id"] as StringTag;
                    if (itemIdTag.Value == searchedItem)
                    {
                        if (results.ContainsKey(position))
                        {
                            results[position] += (itemTag["Count"] as ByteTag).Value;
                        }
                        else
                        {
                            results[position] = (itemTag["Count"] as ByteTag).Value;
                        }
                    }
                }
            }
            
            return results;
        }
    }
}