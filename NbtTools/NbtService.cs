using NbtTools.Nbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
