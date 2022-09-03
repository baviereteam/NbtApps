using SharpNBT;
using System.Collections.Generic;

namespace NbtTools.Nbt
{
    public class NbtFilter
    {
        public ICollection<CompoundTag> GetAllCompoundsWithId(ICollection<CompoundTag> rootTag, string id)
        {
            var tags = new List<CompoundTag>();

            // open all subtags, check id, and only add if it matches
            foreach (Tag t in rootTag)
            {
                if (t is CompoundTag)
                {
                    var compoundTag = (CompoundTag) t;
                    var idTag = compoundTag["id"];

                    if (idTag is StringTag && (idTag as StringTag).Value == id)
                    {
                        tags.Add(compoundTag);
                    }
                }
            }

            return tags;
        }
    }
}
