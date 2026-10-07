using fNbt;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Nbt
{
    public static class NbtFilter
    {
        public static ICollection<Versioned<NbtCompound>> GetAllCompoundsWithId(ICollection<Versioned<NbtCompound>> rootTags, string id)
        {
            return GetAllCompoundsWithId(rootTags, new string[] { id });  
        }

        public static ICollection<Versioned<NbtCompound>> GetAllCompoundsWithId(ICollection<Versioned<NbtCompound>> rootTags, string[] ids)
        {
            var tags = new List<Versioned<NbtCompound>>();

            // open all subtags, check id, and only add if it matches
            foreach (var versionedRootTag in rootTags)
            {
                NbtCompound NbtCompound = versionedRootTag.Tag;
                if (NbtCompound != null)
                {
                    var idTag = NbtCompound["id"] as NbtString;

                    if (idTag != null && ids.Contains(idTag.Value))
                    {
                        tags.Add(versionedRootTag);
                    }
                }
            }

            return tags;
        }
    }
}
