using SharpNBT;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Nbt
{
    public class NbtFilter
    {
        public ICollection<CompoundTag> GetAllCompoundsWithId(ICollection<CompoundTag> rootTag, string id)
        {
            return GetAllCompoundsWithId(rootTag, new string[] { id });  
        }

        public ICollection<CompoundTag> GetAllCompoundsWithId(ICollection<CompoundTag> rootTag, string[] ids)
        {
            var tags = new List<CompoundTag>();

            // open all subtags, check id, and only add if it matches
            foreach (Tag t in rootTag)
            {
                var compoundTag = t as CompoundTag;
                if (compoundTag != null)
                {
                    var idTag = compoundTag["id"] as StringTag;

                    if (idTag != null && ids.Contains(idTag.Value))
                    {
                        tags.Add(compoundTag);
                    }
                }
            }

            return tags;
        }
    }
}
