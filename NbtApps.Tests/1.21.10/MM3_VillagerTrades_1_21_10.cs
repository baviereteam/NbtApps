using McMerchants.Database;
using McMerchants.Models.Database;
using Microsoft.Extensions.DependencyInjection;
using McMerchantsLib.Stock;
using NbtTools.Entities.Trading;

namespace NbtApps.Tests.v1_21_10
{
    [TestClass]
    public class MM3_VillagerTrades_1_21_10 : TestBase
    {
        private const string TEST_DIMENSION = "test_dimension";

        [TestInitialize]
        public void Setup()
        {
            CreateHost(
                new Dictionary<string, string>()
                {
                    { TEST_DIMENSION, Path.Combine(FixturesDirectory, "CombinedTestMap-1.21.10") }
                },
                Path.Combine(FixturesDirectory, "NbtDatabases/nbt_1.21.4.db")
            );

            var dbContext = Host.Services.GetService<McMerchantsDbContext>();

            // create the test store
            dbContext.TradingRegions.Add(new TradingRegion()
            {
                Name = "Test shop",
                Dimension = TEST_DIMENSION,
                StartX = 2,
                StartY = -61,
                StartZ = 6,
                EndX = 8,
                EndY = -57,
                EndZ = 2
            });
            dbContext.SaveChanges();
        }

        [TestMethod]
        public void SearchRegularItem_Found()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:iron_helmet");

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Stores.Count);
            Assert.AreEqual(1, results.Trades.Count);

            var trades = results.Trades.First().Value;
            Assert.AreEqual(1, trades.Count());

            var trade = trades.First();
            Assert.AreEqual("5 Emerald", trade.Buy1.ToString());
            Assert.IsNull(trade.Buy2);
            Assert.AreEqual("1 Iron Helmet", trade.Sell.ToString());
            Assert.AreEqual(0, trade.Sell.Enchantments.Count);
        }

        [TestMethod]
        public void SearchEnchantedItem_Found()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:diamond_boots");

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Stores.Count);
            Assert.AreEqual(1, results.Trades.Count);

            var trades = results.Trades.First().Value;
            Assert.AreEqual(1, trades.Count());

            var trade = trades.First();
            Assert.AreEqual("22 Emerald", trade.Buy1.ToString());
            Assert.IsNull(trade.Buy2);
            Assert.AreEqual("1 Diamond Boots (enchanted)", trade.Sell.ToString());

            Assert.AreEqual(2, trade.Sell.Enchantments.Count);

            var depthStriderEnchantment = new Enchantment("minecraft:depth_strider", 1);
            var featherFallingEnchantment = new Enchantment("minecraft:feather_falling", 3);

            Assert.IsTrue(trade.Sell.Enchantments.Contains(depthStriderEnchantment));
            Assert.IsTrue(trade.Sell.Enchantments.Contains(featherFallingEnchantment));
        }

        [TestCleanup]
        public void TearDown()
        {
            CloseConnection();
        }
    }
}
