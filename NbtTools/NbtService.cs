using NbtTools.Nbt;

namespace NbtTools
{
    public class NbtService
    {
        protected NbtFilter nbtFilter = new NbtFilter();
        protected RegionQueryService regionQuery;

        public NbtService(RegionQueryService regionQuery)
        {
            this.regionQuery = regionQuery;
        }
    }
}
