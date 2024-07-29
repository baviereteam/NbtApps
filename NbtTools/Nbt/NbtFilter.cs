using SharpNBT;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Nbt
{
    public class NbtFilter
    {
        public ICollection<Versioned<CompoundTag>> GetAllCompoundsWithId(ICollection<Versioned<CompoundTag>> rootTags, string id)
        {
            return GetAllCompoundsWithId(rootTags, new string[] { id });  
        }

        public ICollection<Versioned<CompoundTag>> GetAllCompoundsWithId(ICollection<Versioned<CompoundTag>> rootTags, string[] ids)
        {
            var tags = new List<Versioned<CompoundTag>>();

            // open all subtags, check id, and only add if it matches
            foreach (var versionedRootTag in rootTags)
            {
                CompoundTag compoundTag = versionedRootTag.Tag;
                if (compoundTag != null)
                {
                    var idTag = compoundTag["id"] as StringTag;

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
