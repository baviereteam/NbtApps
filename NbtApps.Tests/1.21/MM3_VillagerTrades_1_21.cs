using McMerchants.Database;
using McMerchants.Models.Database;
using Microsoft.Extensions.DependencyInjection;
using McMerchantsLib.Stock;

namespace NbtApps.Tests.v1_21
{
    [TestClass]
    public class MM3_VillagerTrades_1_21 : TestBase
    {
        private const string TEST_DIMENSION = "test_dimension";

        [TestInitialize]
        public void Setup()
        {
            CreateHost(
                new Dictionary<string, string>()
                {
                    { TEST_DIMENSION, Path.Combine(FixturesDirectory, "VillagerTrades-1.21") }
                },
                Path.Combine(FixturesDirectory, "NbtDatabases/nbt_1.21.db")
            );

            var dbContext = Host.Services.GetService<McMerchantsDbContext>();

            // create the test store
            dbContext.TradingRegions.Add(new TradingRegion()
            {
                Name = "Test shop",
                Dimension = TEST_DIMENSION,
                StartX = -2,
                StartY = -61,
                StartZ = -15,
                EndX = -6,
                EndY = -57,
                EndZ = -2
            });
            dbContext.SaveChanges();
        }

        [TestMethod]
        public void SearchRegularItem_Found()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:iron_chestplate");

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Stores.Count);
            Assert.AreEqual(1, results.Trades.Count);

            var trades = results.Trades.First().Value;
            Assert.AreEqual(1, trades.Count());

            var trade = trades.First();
            Assert.AreEqual("9 Emerald", trade.Buy1.ToString());
            Assert.IsNull(trade.Buy2);
            Assert.AreEqual("1 Iron Chestplate", trade.Sell.ToString());
            Assert.AreEqual(0, trade.Sell.Enchantments.Count);
        }

        [TestMethod]
        public void SearchEnchantedItem_Found()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:diamond_helmet");

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Stores.Count);
            Assert.AreEqual(1, results.Trades.Count);

            var trades = results.Trades.First().Value;
            Assert.AreEqual(1, trades.Count());

            var trade = trades.First();
            Assert.AreEqual("23 Emerald", trade.Buy1.ToString());
            Assert.IsNull(trade.Buy2);
            Assert.AreEqual("1 Diamond Helmet (enchanted)", trade.Sell.ToString());

            Assert.AreEqual(2, trade.Sell.Enchantments.Count);
            Assert.AreEqual("minecraft:projectile_protection 3", trade.Sell.Enchantments.ElementAt(0).ToString());
            Assert.AreEqual("minecraft:unbreaking 2", trade.Sell.Enchantments.ElementAt(1).ToString());
        }

        [TestCleanup]
        public void TearDown()
        {
            CloseConnection();
        }
    }
}
