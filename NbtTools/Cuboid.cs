using System;
using System.Collections.Generic;

namespace NbtTools
{
    public class Cuboid
    {
        public Point Start { get; }
        public Point End { get; }

        public Cuboid(Point start, Point end)
        {
            var smallerX = Math.Min(start.X, end.X);
            var smallerY = Math.Min(start.Y, end.Y);
            var smallerZ = Math.Min(start.Z, end.Z);

            Start = new Point(smallerX, smallerY, smallerZ);

            var biggerX = Math.Max(start.X, end.X);
            var biggerY = Math.Max(start.Y, end.Y);
            var biggerZ = Math.Max(start.Z, end.Z);

            End = new Point(biggerX, biggerY, biggerZ);
        }

        public ICollection<Chunk> GetAllChunks()
        {
            var startChunk = Chunk.FromPoint(Start);
            var endChunk = Chunk.FromPoint(End);

            var smallerX = Math.Min(startChunk.X, endChunk.X);
            var smallerZ = Math.Min(startChunk.Z, endChunk.Z);

            var biggerX = Math.Max(startChunk.X, endChunk.X);
            var biggerZ = Math.Max(startChunk.Z, endChunk.Z);

            var chunks = new List<Chunk>();

            for(var x = smallerX; x <= biggerX; x++)
            {
                for (var z = smallerZ; z <= biggerZ; z++)
                {
                    chunks.Add(new Chunk(x, z));
                }
            }

            return chunks;
        }

        public override bool Equals(object obj)
        {
            return obj is Cuboid cuboid &&
                   EqualityComparer<Point>.Default.Equals(Start, cuboid.Start) &&
                   EqualityComparer<Point>.Default.Equals(End, cuboid.End);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        public override string ToString()
        {
            return $"Cuboid from {Start} to {End}";
        }
    }
}
