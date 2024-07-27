using McMerchants.Database;
using McMerchants.Extensions.DependencyInjection;
using McMerchantsLib.Stock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using McMerchants.Models.Database;
using Microsoft.Data.Sqlite;

namespace NbtApps.Tests
{
    [TestClass]
    public class MM1_StoreBoundaries_1_20_1
    {
        private SqliteConnection Connection;

        public string FixturesDirectory { get; }
        private const string TEST_DIMENSION = "test_dimension";
        private StockService StockService;

        public MM1_StoreBoundaries_1_20_1()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FixturesDirectory = Path.Combine(currentPath, "Fixtures");
        }

        [TestInitialize]
        public void Setup()
        {
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();

            var host = Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostContext, configuration) =>
                {
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>() 
                    {
                        { $"MapPaths:{TEST_DIMENSION}", Path.Combine(FixturesDirectory, "StoreAlleyBoundaries-1.20.1")}
                    });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMcMerchantsLib(Connection, new McMerchantsLibOptions
                    {
                        NbtToolsDatabaseConnectionString = null
                    });
                })
                .Build();

            // create the schema in memory
            var dbContext = host.Services.GetService<McMerchantsDbContext>();
            dbContext.Database.EnsureCreated();

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

            StockService = host.Services.GetService<StockService>();
        }

        [TestMethod]
        public void SearchForRedSand()
        {
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
        public void SearchForSlimeBlocks()
        {
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
            Connection.Dispose();
        }

    }
}