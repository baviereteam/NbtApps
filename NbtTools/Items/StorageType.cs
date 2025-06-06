namespace NbtTools.Items
{
    public enum StorageType
    {
        CHEST,
        TRAPPED_CHEST,
        BARREL,
        SHULKERBOX,
        CHISELED_BOOKSHELF
    }

    public static class StorageTypeFactory
    {
        public static StorageType GetStorageType(string blockId)
        {
            switch (blockId)
            {
                case "minecraft:chest":
                    return StorageType.CHEST;
                case "minecraft:trapped_chest":
                    return StorageType.TRAPPED_CHEST;
                case "minecraft:barrel":
                    return StorageType.BARREL;

                case "minecraft:shulker_box":
                    return StorageType.SHULKERBOX;

                case "minecraft:chiseled_bookshelf":
                    return StorageType.CHISELED_BOOKSHELF;

                default:
                    throw new System.Exception($"Tried to create an invalid storage type: {blockId}");
            }
        }

        public static string GetId(this StorageType type)
        {
            return type switch
            {
                StorageType.BARREL => "minecraft:barrel",
                StorageType.CHEST => "minecraft:chest",
                StorageType.TRAPPED_CHEST => "minecraft:trapped_chest",
                StorageType.SHULKERBOX => "minecraft:shulker_box",
                StorageType.CHISELED_BOOKSHELF => "minecraft:chiseled_bookshelf",
                _ => throw new System.Exception($"Unsupported storage type {type}")
            };
        }
    }
}
