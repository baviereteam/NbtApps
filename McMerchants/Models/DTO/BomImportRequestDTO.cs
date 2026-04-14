namespace McMerchants.Models.DTO;

/// <summary>
/// A raw BOM as plain text (the raw contents of a Litematica CSV file)
/// </summary>
public class BomImportRequestDTO
{
    public string Name { get; set; }
    public string Data { get; set; }
}
