using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NbtTools.Database;
using NbtTools.Entities;
using NbtTools.Items;
using NbtTools.Mca;
using System;

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
            services.AddTransient<VillagerService>();
            services.AddTransient<StoredItemService>();

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
