using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using McMerchants.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace McMerchants
{
    public static class Program
    {
        /// <summary>
        /// This authorization policy verifies whether the current user has a claim for being a member of the Discord server
        /// configured in appsettings.
        /// </summary>
        public const string POLICY_IS_IN_DISCORD_SERVER = "InDiscordServer";

        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunWithTasksAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
