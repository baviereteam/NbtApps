using McMerchants.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace McMerchants.Extensions.DependencyInjection
{
    // From https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
            => services.AddTransient<IStartupTask, T>();
    }
}
