using SharpNBT;
using System.Collections.Generic;

namespace NbtTools
{
    internal static class VersionedExtensions
    {
        /// <summary>
        /// Returns a versioned tag containing the child tag with a given key in the versioned parent tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Versioned<T> Get<T>(this Versioned<CompoundTag> container, string key)
            where T : TagContainer
        {
            T tag = container.Tag[key] as T;
            return new Versioned<T>(tag, container.DataVersion);
        }

        /// <summary>
        /// For each child tag contained in the versioned parent tag, returns a versioned tag containing the child tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="versionedTag"></param>
        /// <returns></returns>
        public static IEnumerable<Versioned<Tag>> Enumerate<T>(this Versioned<T> versionedTag)
            where T : TagContainer
        {
            var enumerableTag = versionedTag.Tag;
            foreach (var element in enumerableTag)
            {
                yield return new Versioned<Tag>(element, versionedTag.DataVersion);
            }
        }
    }
}
