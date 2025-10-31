using McMerchants.Database;
using McMerchants.Models.Database;
using Microsoft.Extensions.DependencyInjection;
using McMerchantsLib.Stock;

namespace NbtApps.Tests.v1_21
{
    [TestClass]
    public class MM2_AlleyBoundaries_1_21 : TestBase
    {
        private const string TEST_DIMENSION = "test_dimension";

        [TestInitialize]
        public void Setup()
        {
            CreateHost(
                new Dictionary<string, string>()
                {
                    { TEST_DIMENSION, Path.Combine(FixturesDirectory, "AlleyBoundaries-1.21") }
                },
                Path.Combine(FixturesDirectory, "NbtDatabases/nbt_1.21.db")
            );

            var dbContext = Host.Services.GetService<McMerchantsDbContext>();

            // create the test store
            var store = new StorageRegion()
            {
                Name = "Test store",
                Dimension = TEST_DIMENSION,
                StartX = -16,
                StartY = -61,
                StartZ = 16,
                EndX = -7,
                EndY = -55,
                EndZ = 19
            };
            dbContext.StorageRegions.Add(store);
            store.Alleys =
            [
                new Alley()
                {
                    Name = "Pink alley",
                    Direction = Alley.AlleyDirection.Z,
                    Coordinate = 17,
                    LowBoundary = -12,
                    HighBoundary = -9,
                    StartY = -59,
                    EndY = -57
                },
            ];

            dbContext.SaveChanges();
        }

        [TestMethod]
        public void SearchInAlley_ReturnsAlley()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:honey_block");

            Assert.AreEqual(0, results.Factories.Count);
            Assert.AreEqual(0, results.Trades.Count);
            Assert.AreEqual(1, results.Stores.Count);
            Assert.IsNull(results.Stores[0].StockInDefaultAlley);
            Assert.AreEqual(0, results.Stores[0].StockInBulkContainers.Count);

            int count = 0;
            foreach (var bulk in results.Stores[0].StockInOtherAlleys)
            {
                count += bulk.Value;
            }

            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void SearchOutsideAlley_ReturnsBulk()
        {
            var StockService = Host.Services.GetService<StockService>();
            var results = StockService.GetStockOf("minecraft:ice");

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

            Assert.AreEqual(6, count);
        }

        [TestCleanup]
        public void TearDown()
        {
            CloseConnection();
        }
    }
}
