using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using NbtTools.Extensions.DependencyInjection;
using NbtTools.Database;
using Microsoft.Extensions.Options;
using McMerchants.Database;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using McMerchants.Extensions.DependencyInjection;

namespace McMerchants
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();

            services.AddMcMerchantsLib(new McMerchantsLibOptions
            {
                McMerchantsDatabaseConnectionString = Configuration.GetConnectionString("McMerchantsDatabase"),
                NbtToolsDatabaseConnectionString = Configuration.GetConnectionString("NbtDatabase")
            });

            services
                .AddMvc()
                .AddViewLocalization();

            services.AddTransient<IStringLocalizerFactory, MinecraftIdLocalizerFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var supportedCultures = new[] { "en-US" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Use [Route()] annotations in controllers to manage routing.
            // Prevents conflicts when multiple routes match when using endpoints.MapControllerRoute().
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
