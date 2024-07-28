using Microsoft.VisualBasic;
using SharpNBT;
using System;

namespace NbtTools
{
    /// <summary>
    /// Binds a DataVersion number to a Tag.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Versioned<T> where T : Tag
    {
        public T Tag { get; }
        public int DataVersion { get; }

        public Versioned(T tag, int dataVersion)
        {
            Tag = tag;
            DataVersion = dataVersion;
        }

        public Versioned<T2> As<T2>()
            where T2 : Tag
        {
            if (Tag is T2 convertedTag)
            {
                return new Versioned<T2>(convertedTag, DataVersion);
            }

            // If the conversion isn't possible, the as operator returns null.
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast#as-operator
            return null;
        }
    }
}
