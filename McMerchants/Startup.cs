using McMerchants.Database;
using McMerchants.Extensions.DependencyInjection;
using McMerchants.Json.Bom;
using McMerchants.Json.Stock;
using McMerchants.Services;
using McMerchants.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace McMerchants
{
    public class Startup
    {
        /// <summary>
        /// We keep the connection here, open during the whole application lifetime, to prevent it to be closed during execution,
        /// which would cause the schema to be lost since it's an in-memory database.
        /// </summary>
        private readonly SqliteConnection _connection;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStartupTask<TextureAtlasToCssConverter>();

            services.AddMcMerchantsLib(new McMerchantsLibOptions
            {
                McMerchantsDatabaseConnectionString = Configuration.GetConnectionString("McMerchantsDatabase"),
                NbtToolsDatabaseConnectionString = Configuration.GetConnectionString("NbtDatabase")
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.SlidingExpiration = false;
                });

            services.AddDbContext<OpenIddictDbContext>(options =>
            {
                options.UseSqlite(_connection);
                options.UseOpenIddict();
            });

            services.AddOpenIddict().AddCore(options =>
            {
                options.UseEntityFrameworkCore().UseDbContext<OpenIddictDbContext>();
            })
            .AddClient(options =>
            {
                options.AllowAuthorizationCodeFlow();
                options
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                options
                    .UseAspNetCore()
                    .EnableRedirectionEndpointPassthrough()
                    .EnablePostLogoutRedirectionEndpointPassthrough();

                options.UseSystemNetHttp().SetProductInformation(typeof(Program).Assembly);

                options.UseWebProviders().AddDiscord(options =>
                {
                    options
                        .SetClientId(Configuration["DiscordOAuth:clientId"])
                        .SetClientSecret(Configuration["DiscordOAuth:clientSecret"])
                        .SetRedirectUri("/auth/login/callback")
                        .AddScopes("guilds");
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("InDiscordServer", policy => policy.RequireClaim("IsInGuild"));
            });

            services.AddSingleton<ItemProviderLinksBuilder>();
            services.AddSingleton<PluginApiConverter>();
            services.AddSingleton<WebApiConverter>();
            services.AddSingleton<BomImportResultConverter>();
            services.AddSingleton<BomItemConverter>();

            services.AddCors(options =>
            {
                options.AddPolicy("GET from allowed origins", policy =>
                {
                    policy
                        .WithOrigins(Configuration["Cors:AllowedOrigins"].Split(";"))
                        .WithMethods("GET");
                });
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseCors();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Use [Route()] annotations in controllers to manage routing.
            // Prevents conflicts when multiple routes match when using endpoints.MapControllerRoute().
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Because we're using an in-memory database, make sure it's created.
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OpenIddictDbContext>();
                context.Database.EnsureCreated();
            }
        }
    }
}
