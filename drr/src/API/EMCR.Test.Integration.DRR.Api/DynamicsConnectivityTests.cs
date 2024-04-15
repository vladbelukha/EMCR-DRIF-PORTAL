using EMBC.Tests.Integration.DFA.Api;
using EMCR.DRR.Dynamics;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EMCR.Tests.Integration.DFA.Api
{
    public class DynamicsConnectivityTests
    {
        [Test]
        public async Task GetSecurityToken()
        {
            var host = Application.Host;

            var tokenProvider = host.Services.GetRequiredService<ISecurityTokenProvider>();
            var token = await tokenProvider.AcquireToken();
            Console.WriteLine("Authorization: Bearer " + token);
        }

        [Test]
        public async Task CanConnectToDynamics()
        {
            var host = Application.Host;

            var factory = host.Services.GetRequiredService<IDRRContextFactory>();
            var ctx = factory.Create();

            var results = await ctx.contacts.GetAllPagesAsync();
            results.ShouldNotBeEmpty();
        }
    }
}
