using McMerchants.Database;
using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.DependencyInjection;

namespace NbtApps.Tests
{
    [TestClass]
    public class MM1_StoreBoundaries_1_20_1 : TestBase
    {
        private const string TEST_DIMENSION = "test_dimension";

        [TestInitialize]
        public void Setup()
        {
            CreateHost(
                new Dictionary<string, string>()
                {
                    { TEST_DIMENSION, Path.Combine(FixturesDirectory, "StoreBoundaries-1.20.1") }
                },
                Path.Combine(FixturesDirectory, "NbtDatabases/nbt_1.20.1.db")
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
            var results = StockService.GetStockOf("minecraft:red_sand");

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Trades.Count);
            Assert.AreEqual(1, results.Stores.Count);
            Assert.IsNull(results.Stores[0].StockInDefaultAlley);
            Assert.AreEqual(0, results.Stores[0].StockInOtherAlleys.Count);

            int count = 0;
            foreach (var bulk in results.Stores[0].StockInBulkContainers)
            {
                count += bulk.Value;
            }

            Assert.AreEqual(7, count);
        }

        [TestMethod]
        public void SearchOutsideStore_ReturnsZero()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:slime_block");

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Trades.Count);
            Assert.AreEqual(1, results.Stores.Count);
            Assert.IsNull(results.Stores[0].StockInDefaultAlley);
            Assert.AreEqual(0, results.Stores[0].StockInOtherAlleys.Count);
            Assert.AreEqual(0, results.Stores[0].StockInBulkContainers.Count);
        }

        [TestCleanup]
        public void TearDown()
        {
            CloseConnection();
        }
    }
}