using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EMCR.Utilities.Caching
{
    public static class Configuration
    {
        public static IServiceCollection ConfigureCache(this IServiceCollection services)
        {
            services.AddSingleton(sp => new CacheSyncManager(sp.GetRequiredService<ILogger<CacheSyncManager>>()));
            services.AddSingleton<ICache, Cache>();
            return services;
        }
    }
}
