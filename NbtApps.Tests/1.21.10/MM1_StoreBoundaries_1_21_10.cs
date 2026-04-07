using McMerchants.Database;
using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.DependencyInjection;

namespace NbtApps.Tests.v1_21_10
{
    [TestClass]
    public class MM1_StoreBoundaries_1_21_10 : TestBase
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
            dbContext.StorageRegions.Add(new StorageRegion()
            {
                Name = "Test store",
                Dimension = TEST_DIMENSION,
                StartX = -4,
                StartY = -61,
                StartZ = 23,
                EndX = 7,
                EndY = -56,
                EndZ = 11
            });
            dbContext.SaveChanges();
        }

        [TestMethod]
        public void SearchInsideStore_ReturnsCount()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:red_sand").Results.First().Value;

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Trades.Count);
            Assert.AreEqual(1, results.Stores.Count);
            Assert.IsNull(results.Stores.First().StockInDefaultAlley);
            Assert.AreEqual(0, results.Stores.First().StockInOtherAlleys.Count);

            int count = 0;
            foreach (var bulk in results.Stores.First().StockInBulkContainers)
            {
                count += bulk.Value;
            }

            Assert.AreEqual(7, count);
        }

        [TestMethod]
        public void SearchMultipleItems_ReturnsCount()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:red_sand", "minecraft:purpur_block").Results;
            Assert.AreEqual(2, results.Count);

            var redSandResult = results[results.Keys.Single(k => k.Id == "minecraft:red_sand")];
            Assert.AreEqual(0, redSandResult.Factories.Count);
            Assert.AreEqual(0, redSandResult.Trades.Count);
            Assert.AreEqual(1, redSandResult.Stores.Count);
            Assert.IsNull(redSandResult.Stores.First().StockInDefaultAlley);
            Assert.AreEqual(0, redSandResult.Stores.First().StockInOtherAlleys.Count);

            int count = 0;
            foreach (var bulk in redSandResult.Stores.First().StockInBulkContainers)
            {
                count += bulk.Value;
            }

            Assert.AreEqual(7, count);

            var purpurBlockResult = results[results.Keys.Single(k => k.Id == "minecraft:purpur_block")];
            Assert.AreEqual(0, purpurBlockResult.Factories.Count);
            Assert.AreEqual(0, purpurBlockResult.Trades.Count);
            Assert.AreEqual(1, purpurBlockResult.Stores.Count); // All stores are returned, even empty ones

            var purpurBlockStoreResult = purpurBlockResult.Stores.First();
            Assert.IsNull(purpurBlockStoreResult.StockInDefaultAlley);
            Assert.AreEqual(0, purpurBlockStoreResult.StockInOtherAlleys.Count);
            Assert.AreEqual(0, purpurBlockStoreResult.StockInBulkContainers.Count);
        }

        [TestMethod]
        public void SearchOutsideStore_ReturnsZero()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:slime_block").Results.First().Value;

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Trades.Count);
            Assert.AreEqual(1, results.Stores.Count);
            Assert.IsNull(results.Stores.First().StockInDefaultAlley);
            Assert.AreEqual(0, results.Stores.First().StockInOtherAlleys.Count);
            Assert.AreEqual(0, results.Stores.First().StockInBulkContainers.Count);
        }

        [TestCleanup]
        public void TearDown()
        {
            CloseConnection();
        }
    }
}