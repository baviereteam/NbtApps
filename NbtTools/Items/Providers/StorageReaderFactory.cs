namespace NbtTools.Items.Providers
{
    /// <summary>
    /// Provides a reader to access data about item storage for a given Data Version.
    /// </summary>
    public class StorageReaderFactory
    {
        private readonly Version3837StorageReader v3837Provider = new Version3837StorageReader();
        private readonly StorageReader previousVersionsProvider = new StorageReader();

        internal StorageReader GetForVersion(int chunkDataVersion)
        {
            if (chunkDataVersion >= 3837)
            {
                return v3837Provider;
            }

            return previousVersionsProvider;
        }
    }
}
