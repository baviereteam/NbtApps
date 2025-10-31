using System.Collections.Generic;

namespace NbtTools.Entities.Providers
{
    public class EntityReaderFactory
    {
        private readonly IDictionary<string, EntityReader> providers = new Dictionary<string, EntityReader>();

        public EntityReader GetForVersion(int chunkDataVersion)
        {
            if (chunkDataVersion >= 4556)
            {
                return GetOrCreateReader<Version4556EntityReader>();
            }
            if (chunkDataVersion >= 3837)
            {
                return GetOrCreateReader<Version3837EntityReader>();
            }

            return GetOrCreateReader<EntityReader>();
        }

        private EntityReader GetOrCreateReader<T>() where T : EntityReader, new()
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
