namespace McMerchants.Models.Database
{
    /// <summary>
    /// An "alley" in a "store" (StorageRegion), to give a "swedish furniture warehouse" feel...
    /// </summary>
    public class Alley
    {
        public enum AlleyDirection
        {
            X,
            Z
        };

        public int Id { get; set; }

        public StorageRegion Store { get; set; }
        public int StoreId { get; set; }

        public AlleyDirection Direction { get; set; }

        /// <summary>
        /// The coordinate of the alley, on the opposite axis designated by <c>Direction</c>.
        /// </summary>
        public int Coordinate { get; set; }

        /// <summary>
        /// The coordinate at which this alley starts, on the axis designated by <c>Direction</c>.
        /// </summary>
        public int LowBoundary { get; set; }

        /// <summary>
        /// The coordinate at which this alley ends, on the axis designated by <c>Direction</c>.
        /// </summary>
        public int HighBoundary { get; set; }

        public int StartY { get; set; }
        public int EndY { get; set; }

        public string Name { get; set; }
    }
}
