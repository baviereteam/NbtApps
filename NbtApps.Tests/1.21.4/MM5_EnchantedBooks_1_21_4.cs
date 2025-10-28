using McMerchants.Database;
using McMerchants.Models.Database;
using McMerchantsLib.Stock;
using Microsoft.Extensions.DependencyInjection;

namespace NbtApps.Tests.v1_21_4;

[TestClass]
public class MM5_EnchantedBooks_1_21_4 : TestBase
{
    private const string TEST_DIMENSION = "test_dimension";

    [TestInitialize]
    public void Setup()
    {
        CreateHost(
            new Dictionary<string, string>()
            {
                    { TEST_DIMENSION, Path.Combine(FixturesDirectory, "PotionsAndEnchantedBooks-1.21.4") }
            },
            Path.Combine(FixturesDirectory, "NbtDatabases/nbt_1.21.4.db")
        );

        var dbContext = Host.Services.GetService<McMerchantsDbContext>();

        // create the test store
        dbContext.StorageRegions.Add(new StorageRegion()
        {
            Name = "Test store",
            Dimension = TEST_DIMENSION,
            StartX = 1,
            StartY = -61,
            StartZ = 1,
            EndX = 3,
            EndY = -59,
            EndZ = 3
        });
        dbContext.SaveChanges();
    }

    [TestMethod]
    public void SearchForEnchantedBooks_ReturnsEnchantedBooksInChest()
    {
        var StockService = Host.Services.GetService<StockService>();

        var results = StockService.GetStockOf("enchanted_book:wind_burst_1");
        Assert.AreEqual(1, results.Stores.Count);
        Assert.AreEqual(0, results.Factories.Count);
        Assert.AreEqual(0, results.Trades.Count);
        Assert.AreEqual(1, results.Stores[0].StockInBulkContainers.First().Value);
    }

    [TestMethod]
    public void SearchForEnchantedBooks_ReturnsEnchantedBooksInShulkerInChest()
    {
        var StockService = Host.Services.GetService<StockService>();

        var results = StockService.GetStockOf("enchanted_book:protection_3");
        Assert.AreEqual(1, results.Stores.Count);
        Assert.AreEqual(0, results.Factories.Count);
        Assert.AreEqual(0, results.Trades.Count);
        Assert.AreEqual(1, results.Stores[0].StockInBulkContainers.First().Value);
    }

    [TestCleanup]
    public void TearDown()
    {
        CloseConnection();
    }
}
