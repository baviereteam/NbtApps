using NbtTools.Geography;

namespace McMerchants.Models
{
    public class Store
    {
        public Store(string name, string logo, string dimension, Point start, Point end)
        {
            Name = name;
            Logo = logo;
            Coordinates = new Cuboid(dimension, start, end);
        }

        public string Name { get; set; }
        public string Logo { get; set; }
        public Cuboid Coordinates { get; set; }
    }
}
