using System;

namespace NbtTools.Geography
{
    public class Chunk
    {
        public double X { get; private set; }
        public double Z { get; private set; }
        public Region Region { get; private set; }

        public Chunk()
        {

        }

        public Chunk(double x, double z)
        {
            X = x;
            Z = z;
            Region = Region.FromPoint(new Point(x*16, 0, z*16));
        }

        public static Chunk FromPoint(Point p)
        {
            return new Chunk
            {
                X = Math.Floor(p.X / 16.0),
                Z = Math.Floor(p.Z / 16.0),
                Region = Region.FromPoint(p)
            };
        }

        public override string ToString()
        {
            return $"Chunk ({X},{Z})";
        }

        public override bool Equals(object obj)
        {
            return obj is Chunk chunk &&
                   X == chunk.X &&
                   Z == chunk.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }
    }
}
