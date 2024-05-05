using NbtTools.Geography;

namespace McMerchants.Models.Database
{
    /// <summary>
    /// Represents a place (cuboid) enriched with display properties.
    /// </summary>
    /**
     * Uses EFCore type inheritance.
     * See https://learn.microsoft.com/en-us/ef/core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration.
    **/
    public class ItemProviderRegion
    {
        private Cuboid _coordinates;

        public string Type { get; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string Dimension { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int StartZ { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public int EndZ { get; set; }

        public Cuboid Coordinates
        {
            get
            {
                if (_coordinates == null)
                {
                    _coordinates = new Cuboid(
                        Dimension,
                        new Point(StartX, StartY, StartZ),
                        new Point(EndX, EndY, EndZ)
                    );
                }

                return _coordinates;
            }
        }
    }
}
