using System.Collections.Generic;
using System.Numerics;

namespace NbtTools
{
    public static class DictionaryExtensions
    {
        extension<K, V>(IDictionary<K, V> source) where V : INumber<V>
        {
            /// <summary>
            /// Adds a new key-value pair if the key does not exist, or increments the value if the key exists.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void AddOrIncrement(K key, V value)
            {
                if (source.ContainsKey(key))
                {
                    source[key] += value;
                }
                else
                {
                    source.Add(key, value);
                }
            }

            /// <summary>
            /// Appends the content of another dictionary of numbers to this one.
            /// When a key from the other dictionary is present in this one, the values are added.
            /// When the key is not present, the key and value are appended to this dictionary.
            /// </summary>
            /// <param name="other"></param>
            public void AddRange(IDictionary<K, V> other)
            {
                foreach (var pair in other)
                {
                    source.AddOrIncrement(pair.Key, pair.Value);
                }
            }
        }
    }
}