using System.Text.Json.Serialization;

namespace McMerchantsLib.Models.Database
{
    public class BomItem
    {
        [JsonIgnore]
        public int Id { get; init; }
        public required string ItemName { get; set; }
        public required int RequiredQuantity { get; set; }

        [JsonIgnore]
        public int BomId { get; init; }
        [JsonIgnore]
        public required Bom Bom { get; init; }
    }
}
