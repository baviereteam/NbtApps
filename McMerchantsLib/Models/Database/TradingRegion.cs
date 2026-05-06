namespace McMerchants.Models.Database
{
    /// <summary>
    /// Represents a place (cuboid) with multiple villagers whose trades can be consulted (a village, for example).
    /// </summary>
    public class TradingRegion : ItemProviderRegion
    {
        public const string TYPE_KEY = "trading";
        public new string Type { get; } = TYPE_KEY;
    }
}
