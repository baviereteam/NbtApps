using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using McMerchants.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace McMerchants
{
    public class Program
    {
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
