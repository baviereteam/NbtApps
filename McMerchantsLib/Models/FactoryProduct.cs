namespace McMerchants.Models.Database
{
    /// <summary>
    /// Represents an item produced by a factory.
    /// </summary>
    public class FactoryProduct
    {
        public int Id { get; set; }

        public string Item { get; private set; }
        
        public FactoryRegion Factory { get; set; }

        public FactoryProduct(string item)
        {
            Item = item;
        }
    }
}
