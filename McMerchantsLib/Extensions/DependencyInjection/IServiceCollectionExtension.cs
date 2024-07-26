using McMerchants.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NbtTools.Extensions.DependencyInjection;

namespace McMerchants.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddMcMerchantsLib(this IServiceCollection services, McMerchantsLibOptions options)
        {
            services.AddNbtTools(new NbtToolsOptions
            {
                DatabaseConnectionString = options.NbtToolsDatabaseConnectionString
            });

            services.AddDbContext<McMerchantsDbContext>(
            dbOptions => dbOptions.UseSqlite(
                    options.McMerchantsDatabaseConnectionString,
                    sqLiteOptions => sqLiteOptions.MigrationsAssembly("McMerchantsLib")
                )
            );

            return services;
        }
    }
}
