using SharpNBT;

namespace NbtTools
{
    public class VersionedTagContainer<T> where T : Tag
    {
        public T Tag {  get; }
        public int DataVersion {  get; }

        public VersionedTagContainer(T tag, int dataVersion)
        {
            Tag = tag;
            DataVersion = dataVersion;
        }
    }
}
