using fNbt;
using Microsoft.Extensions.Logging;
using NbtTools.Database;
using NbtTools.Entities.Providers;
using NbtTools.RegionQuery;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities.Trading
{
    public class TradeService
    {
        private readonly ILogger<TradeService> Logger;
        private readonly NbtDbContext NbtContext;
        private readonly EntityReaderFactory EntityReaderFactory;

        public TradeService(NbtDbContext context, EntityReaderFactory entityReaderFactory, ILogger<TradeService> logger)
        {
            NbtContext = context;
            EntityReaderFactory = entityReaderFactory;
            Logger = logger;
        }

        public ICollection<Trade> FromRecipesTag(Villager villager, Versioned<NbtList> recipesTag)
        {
            var trades = new List<Trade>();

            foreach (var recipe in recipesTag.Enumerate())
            {
                try
                {
                    trades.Add(FromTradeTag(villager, recipe.As<NbtCompound>()));
                }

                // item does not exist in the NBT database (maybe it's not up to date with Minecraft)
                catch (KeyNotFoundException e)
                {
                    Logger.LogError(e, "Could not create a trade.");
                }
            }

            return trades;
        }

        public Trade FromTradeTag(Villager villager, Versioned<NbtCompound> versionedRootTag) {
            try
            {
                var buy1 = TradeComponentFromTag(versionedRootTag.Get<NbtCompound>("buy"));
                var buy2 = TradeComponentFromTag(versionedRootTag.Get<NbtCompound>("buyB"));
                var sell = TradeComponentFromTag(versionedRootTag.Get<NbtCompound>("sell"));

                return new Trade(villager, buy1, buy2, sell);
            }

            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Could not create trade", e);
            }
        }

        public TradeComponent? TradeComponentFromTag(Versioned<NbtCompound> versionedRootTag)
        {
            if (versionedRootTag == null)
            {
                return null;
            }

            try
            {
                var id = (versionedRootTag.Tag["id"] as NbtString).Value;
                if (id == "minecraft:air")
                {
                    return null;
                }

                var item = NbtContext.Items.Find(id);
                if (item == null)
                {
                    throw new KeyNotFoundException($"Item {id} did not exist in the NBT database.");
                }

                var entityReader = EntityReaderFactory.GetForVersion(versionedRootTag.DataVersion);
                var count = entityReader.GetCountFromItemTag(versionedRootTag.Tag);
                var enchantments = entityReader.GetEnchantmentsFromTradeComponent(versionedRootTag.Tag);
                return new TradeComponent(item, count, enchantments);
            }

            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Could not create trade component", e);
            }
        }
    }
}
