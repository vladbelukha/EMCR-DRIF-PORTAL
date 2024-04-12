using System;
using System.Net.Http;
using System.Threading.Tasks;
using EMCR.Utilities;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace EMCR.DRR.Dynamics
{
    public interface ISecurityTokenProvider
    {
        Task<string> AcquireToken();
    }

    internal class ADFSSecurityTokenProvider : ISecurityTokenProvider
    {
        private const string cacheKey = "DRR_dynamics_adfs_token";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IDistributedCache cache;
        private readonly DRRContextOptions options;

        public ADFSSecurityTokenProvider(
            IHttpClientFactory httpClientFactory,
            IOptions<DRRContextOptions> options,
            IDistributedCache cache)
        {
            this.httpClientFactory = httpClientFactory;
            this.cache = cache;
            this.options = options.Value;
        }

        public async Task<string> AcquireToken() => await cache.GetOrSet(cacheKey, AcquireTokenInternal, TimeSpan.FromMinutes(5)) ?? string.Empty;

        private async Task<string> AcquireTokenInternal()
        {
            using var httpClient = httpClientFactory.CreateClient("adfs_token");

            var response = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = options.Adfs.OAuth2TokenEndpoint.AbsoluteUri,
                ClientId = options.Adfs.ClientId,
                ClientSecret = options.Adfs.ClientSecret,
                Resource = { options.Adfs.ResourceName },
                UserName = $"{options.Adfs.ServiceAccountDomain}\\{options.Adfs.ServiceAccountName}",
                Password = options.Adfs.ServiceAccountPassword,
                Scope = "openid",
            });

            if (response.IsError) throw new InvalidOperationException(response.Error);

            return response.AccessToken;
        }
    }
}
