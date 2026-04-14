using DbBom = McMerchantsLib.Models.Database.Bom;

namespace McMerchantsLib.Models.Bom
{
    /// <summary>
    /// Represents the import results of a BOM from CSV text.
    /// </summary>
    public class BomImportResultDTO
    {
        public DbBom Bom { get; set; }
        public ICollection<Tuple<string, string>> UnreadableLines { get; set; } = new List<Tuple<string, string>>();
    }
}
