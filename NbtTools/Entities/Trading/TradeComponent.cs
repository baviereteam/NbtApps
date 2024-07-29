using NbtTools.Items;
using System.Collections.Generic;

namespace NbtTools.Entities.Trading
{
    public class TradeComponent
    {
        public Item Item { get; private set; }
        public int Quantity { get; private set; }
        public ICollection<Enchantment> Enchantments { get; private set; }

        public TradeComponent(Item item, int quantity, ICollection<Enchantment> enchantments)
        {
            Item = item;
            Quantity = quantity;
            Enchantments = enchantments;
        }

        public override string ToString()
        {
            return $"{Quantity} {Item.Name}" + (Enchantments.Count > 0 ? " (enchanted)" : "");
        }
    }
}
