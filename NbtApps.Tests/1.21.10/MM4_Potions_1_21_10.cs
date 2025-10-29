using McMerchants.Database;
using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.DependencyInjection;

namespace NbtApps.Tests.v1_21_10;

[TestClass]
public class MM4_Potions_1_21_10 : TestBase
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
            StartX = 10,
            StartY = -61,
            StartZ = 4,
            EndX = 12,
            EndY = -59,
            EndZ = 6
        });
        dbContext.SaveChanges();
    }

    [TestMethod]
    public void SearchForPotions_ReturnsPotionsInChest()
    {
        var StockService = Host.Services.GetService<StockService>();

        var results = StockService.GetStockOf("potion:invisibility");
        Assert.AreEqual(1, results.Stores.Count);
        Assert.AreEqual(0, results.Factories.Count);
        Assert.AreEqual(0, results.Trades.Count);
        Assert.AreEqual(1, results.Stores[0].StockInBulkContainers.First().Value);

        results = StockService.GetStockOf("potion:lingering_long_strength");
        Assert.AreEqual(1, results.Stores.Count);
        Assert.AreEqual(0, results.Factories.Count);
        Assert.AreEqual(0, results.Trades.Count);
        Assert.AreEqual(1, results.Stores[0].StockInBulkContainers.First().Value);
    }

    [TestMethod]
    public void SearchForPotions_ReturnsPotionsInShulkerInChest()
    {
        var StockService = Host.Services.GetService<StockService>();

        var results = StockService.GetStockOf("potion:splash_long_night_vision");
        Assert.AreEqual(1, results.Stores.Count);
        Assert.AreEqual(0, results.Factories.Count);
        Assert.AreEqual(0, results.Trades.Count);
        Assert.AreEqual(2, results.Stores[0].StockInBulkContainers.First().Value);
    }

    [TestCleanup]
    public void TearDown()
    {
        CloseConnection();
    }
}
