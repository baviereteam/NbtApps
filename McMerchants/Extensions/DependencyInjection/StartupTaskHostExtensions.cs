using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using McMerchants.Tasks;
using Microsoft.Extensions.Hosting;

namespace McMerchants.Extensions.DependencyInjection
{
    // From https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
    public static class StartupTaskHostExtensions
    {
        public static async Task RunWithTasksAsync(this IHost host, CancellationToken cancellationToken = default)
        {
            // Load all tasks from DI
            var startupTasks = host.Services.GetServices<IStartupTask>();

            // Execute all the tasks
            foreach (var startupTask in startupTasks)
            {
                await startupTask.ExecuteAsync(cancellationToken);
            }

            // Start the tasks as normal
            await host.RunAsync(cancellationToken);
        }

    }
}
