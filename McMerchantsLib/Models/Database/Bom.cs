namespace McMerchantsLib.Models.Database
{
    public class Bom
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<BomItem> Items { get; set; }
    }
}
