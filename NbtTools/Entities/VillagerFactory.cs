using NbtTools.Entities.Trading;
using NbtTools.Geography;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities
{
    public class VillagerFactory
    {
        public static Villager FromNbtTag(CompoundTag rootTag)
        {
            try
            {
                ListTag positionTag = rootTag["Pos"] as ListTag;
                double x = (positionTag[0] as DoubleTag).Value;
                double y = (positionTag[1] as DoubleTag).Value;
                double z = (positionTag[2] as DoubleTag).Value;
                Point position = new Point(x, y, z);

                CompoundTag villagerDataTag = rootTag["VillagerData"] as CompoundTag;
                int level = (villagerDataTag["level"] as IntTag).Value;
                string profession = (villagerDataTag["profession"] as StringTag).Value;
                string type = (villagerDataTag["type"] as StringTag).Value;

                ICollection<Trade> trades;
                if (profession != "minecraft:none" && profession != "minecraft:nitwit")
                {
                    ListTag recipes = (rootTag["Offers"] as CompoundTag)["Recipes"] as ListTag;
                    trades = TradeFactory.FromRecipesTag(recipes);
                } else
                {
                    trades = new List<Trade>();
                }
                
                return new Villager(profession, level, type, position, trades);
            }

            catch (Exception e)
            {
                throw new Exception("Could not create villager", e);
            }
        }
    }
}
