using System;

namespace NbtTools
{
    public class Region
    {
        public double X { get; private set; }
        public double Z { get; private set; }
        public static Region FromPoint(Point p)
        {
            var chunkX = Math.Floor(p.X / 16.0);
            var chunkZ = Math.Floor(p.Z / 16.0);

            return new Region
            {
                X = Math.Floor(chunkX / 32.0),
                Z = Math.Floor(chunkZ / 32.0)
            };
        }

        public override bool Equals(object obj)
        {
            return obj is Region region &&
                   X == region.X &&
                   Z == region.Z;
        }

        public string GetFileName()
        {
            return $"r.{X}.{Z}.mca";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }

        public override string ToString()
        {
            return this.GetFileName();
        }
    }
}
