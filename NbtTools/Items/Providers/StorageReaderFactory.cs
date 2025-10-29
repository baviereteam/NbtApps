using System.Collections.Generic;

namespace NbtTools.Items.Providers
{
    /// <summary>
    /// Provides a reader to access data about item storage for a given Data Version.
    /// </summary>
    public class StorageReaderFactory
    {
        private readonly IDictionary<string, StorageReader> providers = new Dictionary<string, StorageReader>();

        internal StorageReader GetForVersion(int chunkDataVersion)
        {
            if (chunkDataVersion >= 4556)
            {
                return GetOrCreateReader<Version4556StorageReader>();
            }
            if (chunkDataVersion >= 3837)
            {
                return GetOrCreateReader<Version3837StorageReader>();
            }

            return GetOrCreateReader<StorageReader>();
        }

        private StorageReader GetOrCreateReader<T>() where T : StorageReader, new()
        {
            string providerClass = typeof(T).Name;
            if (!providers.ContainsKey(providerClass))
            {
                providers.Add(providerClass, new T());
            }

            return providers[providerClass];
        }
    }
}
