using McMerchants.Database;
using McMerchants.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace NbtApps.Tests
{
    public class TestBase
    {
        private SqliteConnection Connection;
        public IHost Host;

        public string FixturesDirectory { get; }

        public TestBase()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FixturesDirectory = Path.Combine(currentPath, "Fixtures");
        }

        private void CreateConnection()
        {
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();
        }

        public void CreateHost(IDictionary<string, string> mapPaths) 
        {
            CreateHost(mapPaths, null);
        }

        public void CreateHost(IDictionary<string, string> mapPaths, string? nbtDatabasePath)
        {
            if (nbtDatabasePath != null)
            {
                nbtDatabasePath = $"Filename={nbtDatabasePath}";
            }

            CreateConnection();

            var configValues = new Dictionary<string, string?>();
            foreach (var mapPath in mapPaths)
            {
                configValues.Add($"MapPaths:{mapPath.Key}", mapPath.Value);
            }

            Host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostContext, configuration) =>
                {
                    configuration.AddInMemoryCollection(configValues);
                })

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMcMerchantsLib(Connection, new McMerchantsLibOptions
                    {
                        NbtToolsDatabaseConnectionString = nbtDatabasePath
                    });
                })
                .Build();

            // create the schema in memory
            var dbContext = Host.Services.GetService<McMerchantsDbContext>();
            dbContext.Database.EnsureCreated();
        }

        public void CloseConnection()
        {
            Connection.Dispose();
        }
    }
}
