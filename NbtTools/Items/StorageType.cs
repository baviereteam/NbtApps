namespace NbtTools.Items
{
    public enum StorageType
    {
        CHEST,
        BARREL,
        SHULKERBOX
    }

    public static class StorageTypeFactory
    {
        public static StorageType GetStorageType(string blockId)
        {
            switch (blockId)
            {
                case "minecraft:chest":
                    return StorageType.CHEST;
                case "minecraft:barrel":
                    return StorageType.BARREL;

                case "minecraft:shulker_box":
                    return StorageType.SHULKERBOX;

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
                StorageType.SHULKERBOX => "minecraft:shulker_box",
                _ => throw new System.Exception($"Unsupported storage type {type}")
            };
        }
    }
}
