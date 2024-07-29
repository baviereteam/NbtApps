namespace NbtTools.Entities.Providers
{
    public class EntityReaderFactory
    {
        private readonly Version3837EntityReader v3837Provider = new Version3837EntityReader();
        private readonly EntityReader previousVersionsProvider = new EntityReader();

        public EntityReader GetForVersion(int chunkDataVersion)
        {
            if (chunkDataVersion >= 3837)
            {
                return v3837Provider;
            }

            return previousVersionsProvider;
        }
    }
}
