using Microsoft.Extensions.Logging;
using NbtTools.Geography;
using NbtTools.Mca;
using fNbt;

namespace NbtTools.RegionQuery
{
    public class EntitiesQuery : AbstractQuery
    {
        private readonly McaFileFactory McaFileFactory;
        protected override string ElementKey => "Entities";

        public EntitiesQuery(McaFileFactory mcaFileFactory, ILogger<EntitiesQuery> logger) : base(logger) 
        {
            this.McaFileFactory = mcaFileFactory;
        }

        protected override McaFile GetFile(string dimension, string fileName)
        {
            return McaFileFactory.GetEntitiesFile(dimension, fileName);
        }

        protected override bool IsInZone(NbtCompound element, Cuboid zone)
        {
            var positionTag = element["Pos"] as NbtList;

            Point position = new Point(
                (positionTag[0] as NbtDouble).Value,
                (positionTag[1] as NbtDouble).Value,
                (positionTag[2] as NbtDouble).Value
            );

            return zone.Contains(position);
        }

        protected override bool IsValidChunk(NbtCompound chunkMainTag)
        {
            return true;
        }
    }
}
