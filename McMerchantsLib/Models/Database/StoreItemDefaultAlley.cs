namespace McMerchants.Models.Database
{
    public class StoreItemDefaultAlley
    {
        public int Id { get; set; }

        public string Item { get; private set; }

        public Alley Alley { get; set; }
    }
}
