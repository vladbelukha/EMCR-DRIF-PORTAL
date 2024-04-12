using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Extensions.Client;

namespace EMCR.DRR.Dynamics
{
    public static class Configuration
    {
        public static IServiceCollection AddDRRDynamics(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("Dynamics").Get<DRRContextOptions>();

            services.Configure<DRRContextOptions>(opts => configuration.GetSection("Dynamics").Bind(opts));

            services
                .AddHttpClient("adfs_token")
                .SetHandlerLifetime(TimeSpan.FromMinutes(30))
               ;

            services.AddSingleton<ISecurityTokenProvider, ADFSSecurityTokenProvider>();

            services
                .AddODataClient("DRR_dynamics")
                .AddODataClientHandler<DynamicsODataClientHandler>()
                .AddHttpClient()
                .ConfigureHttpClient(c => c.Timeout = options.HttpClientTimeout)
                .SetHandlerLifetime(TimeSpan.FromMinutes(30))
                ;

            services.AddSingleton<IDRRContextFactory, DRRContextFactory>();

            return services;
        }
    }

    public class DRRContextOptions
    {
        public Uri DynamicsApiEndpoint { get; set; } = null!;
        public Uri DynamicsApiBaseUri { get; set; } = null!;
        public AdfsOptions Adfs { get; set; } = new AdfsOptions();
        public TimeSpan HttpClientTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }

    public class AdfsOptions
    {
        public Uri OAuth2TokenEndpoint { get; set; } = null!;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string ServiceAccountDomain { get; set; } = string.Empty;
        public string ServiceAccountName { get; set; } = string.Empty;
        public string ServiceAccountPassword { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
    }
}
