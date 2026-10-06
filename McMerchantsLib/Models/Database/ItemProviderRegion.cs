using NbtTools.Geography;
using System.ComponentModel.DataAnnotations;

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
        private Cuboid? _coordinates;

        public string Type { get; }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Logo { get; set; }

        [Required]
        public string Dimension { get; set; }

        public string? URL { get; set; }

        [Required]
        public int StartX { get; set; }

        [Required]
        public int StartY { get; set; }

        [Required]
        public int StartZ { get; set; }

        [Required]
        public int EndX { get; set; }

        [Required]
        public int EndY { get; set; }

        [Required]
        public int EndZ { get; set; }

        public Cuboid Coordinates
        {
            get
            {
                if (_coordinates == null)
                {
                    if (Dimension == null)
                    {
                        return null;
                    }

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
