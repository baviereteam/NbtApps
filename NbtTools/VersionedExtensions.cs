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
        /// <returns>A Versioned<typeparamref name="T"/> containing the child tag with that key, or <c>null</c> if the key was not present in the container tag.</returns>
        public static Versioned<T> Get<T>(this Versioned<CompoundTag> container, string key)
            where T : Tag
        {
            if (container.Tag.ContainsKey(key))
            {
                return new Versioned<T>(container.Tag[key] as T, container.DataVersion);
            }

            return null;
        }

        /// <summary>
        /// For each child tag contained in the versioned parent tag, returns a versioned tag containing the child tag.
        /// </summary>
        /// <param name="versionedTag"></param>
        /// <returns></returns>
        public static IEnumerable<Versioned<Tag>> Enumerate(this Versioned<ListTag> versionedTag)
        {
            var enumerableTag = versionedTag.Tag;
            foreach (var element in enumerableTag)
            {
                yield return new Versioned<Tag>(element, versionedTag.DataVersion);
            }
        }
        /// <summary>
        /// For each child tag contained in the versioned parent tag, returns a versioned tag containing the child tag.
        /// </summary>
        /// <param name="versionedTag"></param>
        /// <returns></returns>
        public static IEnumerable<Versioned<Tag>> Enumerate(this Versioned<CompoundTag> versionedTag)
        {
            var enumerableTag = versionedTag.Tag;
            foreach (var element in enumerableTag)
            {
                yield return new Versioned<Tag>(element, versionedTag.DataVersion);
            }
        }
    }
}
