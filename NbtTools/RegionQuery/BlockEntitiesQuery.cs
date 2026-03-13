using Microsoft.Extensions.Logging;
using NbtTools.Geography;
using NbtTools.Mca;
using SharpNBT;

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

        protected override bool IsInZone(CompoundTag element, Cuboid zone)
        {
            Point position = new Point(
                (element["x"] as IntTag).Value,
                (element["y"] as IntTag).Value,
                (element["z"] as IntTag).Value
            );

            return zone.Contains(position);
        }

        protected override bool IsValidChunk(CompoundTag chunkMainTag)
        {
            var status = chunkMainTag["Status"] as StringTag;
            return (status != null && status == "minecraft:full");
        }
    }
}
