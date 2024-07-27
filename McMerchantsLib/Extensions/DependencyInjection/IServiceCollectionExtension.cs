using McMerchants.Database;
using McMerchantsLib.Stock;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NbtTools.Extensions.DependencyInjection;

namespace McMerchants.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddMcMerchantsLib(this IServiceCollection services, McMerchantsLibOptions options)
        {
            return services.AddMcMerchantsLib(null, options);
        }

        public static IServiceCollection AddMcMerchantsLib(this IServiceCollection services, SqliteConnection connection, McMerchantsLibOptions options)
        {
            services.AddNbtTools(new NbtToolsOptions
            {
                DatabaseConnectionString = options.NbtToolsDatabaseConnectionString
            });

            if (connection == null)
            {
                services.AddDbContext<McMerchantsDbContext>(
                    dbOptions => dbOptions.UseSqlite(
                        options.McMerchantsDatabaseConnectionString,
                        sqLiteOptions => sqLiteOptions.MigrationsAssembly("McMerchantsLib")
                    )
                );
            }
            else
            {
                services.AddDbContext<McMerchantsDbContext>(
                dbOptions => dbOptions.UseSqlite(
                    connection,
                    sqLiteOptions => sqLiteOptions.MigrationsAssembly("McMerchantsLib")
                )
            );
            }
            
            services.AddTransient<StockService>();
            return services;
        }
    }
}
