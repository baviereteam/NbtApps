using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using NbtTools;
using NbtTools.Database;
using NbtTools.Mca;

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
            services.AddNbtTools();
            services.AddLocalization();

            services
                .AddMvc()
                .AddViewLocalization();

            services.AddTransient<IStringLocalizerFactory, MinecraftIdLocalizerFactory>();

            services.AddDbContext<NbtDbContext>(
                options => options.UseSqlite(
                    Configuration.GetConnectionString("NbtDatabase"),
                    sqLiteOptions => sqLiteOptions.MigrationsAssembly("NbtTools")
                )
            );
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

            app.UseEndpoints(endpoints =>
            {
                endpoints
                .MapControllerRoute(
                    name: "trades",
                    pattern: "{controller=Shop}/{action=Details}/{fromX}/{fromY}/{fromZ}/{toX}/{toY}/{toZ}"
                );
                endpoints
                .MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Item}/{action=Details}/{id?}"
                );

                endpoints.MapControllers();
            });
        }
    }
}
