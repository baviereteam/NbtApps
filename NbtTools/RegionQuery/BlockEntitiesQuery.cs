using Microsoft.Extensions.Logging;
using NbtTools.Geography;
using NbtTools.Mca;
using fNbt;

namespace NbtTools.RegionQuery
{
    public class BlockEntitiesQuery : AbstractQuery
    {
        private readonly McaFileFactory McaFileFactory;

        protected override string ElementKey => "block_entities";

        public BlockEntitiesQuery(McaFileFactory mcaFileFactory, ILogger<BlockEntitiesQuery> logger) : base(logger)
        {
            this.McaFileFactory = mcaFileFactory;
        }

        protected override McaFile GetFile(string dimension, string fileName)
        {
            return McaFileFactory.GetRegionFile(dimension, fileName);
        }

        protected override bool IsInZone(NbtCompound element, Cuboid zone)
        {
            Point position = new Point(
                (element["x"] as NbtInt).Value,
                (element["y"] as NbtInt).Value,
                (element["z"] as NbtInt).Value
            );

            return zone.Contains(position);
        }

        protected override bool IsValidChunk(NbtCompound chunkMainTag)
        {
            var status = chunkMainTag["Status"] as NbtString;
            return (status != null && status.Value == "minecraft:full");
        }
    }
}
