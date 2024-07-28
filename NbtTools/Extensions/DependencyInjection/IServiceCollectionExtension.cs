using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NbtTools.Database;
using NbtTools.Entities;
using NbtTools.Entities.Trading;
using NbtTools.Items;
using NbtTools.Items.Providers;
using NbtTools.Mca;

namespace NbtTools.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddNbtTools(this IServiceCollection services, NbtToolsOptions options)
        {
            //https://mcguirev10.com/2018/01/31/net-core-class-library-dependency-injection.html
            //TODO: use https://learn.microsoft.com/en-us/dotnet/core/extensions/options to get the DbContext options?
            // Configuration doesn't change during app lifecycle.
            services.AddSingleton<McaFileFactory>();

            services.AddTransient<RegionQueryService>();
            services.AddTransient<StorageReaderFactory>();
            services.AddTransient<VillagerService>();
            services.AddTransient<TradeService>();
            services.AddTransient<StoredItemService>();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddDbContext<NbtDbContext>(
                dbOptions => dbOptions.UseSqlite(
                    options.DatabaseConnectionString,
                    sqLiteOptions => sqLiteOptions.MigrationsAssembly("NbtTools")
                )
            );

            return services;
        }
    }
}
