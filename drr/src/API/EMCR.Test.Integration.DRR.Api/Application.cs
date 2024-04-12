using Alba;
using Microsoft.Extensions.DependencyInjection;

namespace EMBC.Tests.Integration.DFA.Api
{
    [SetUpFixture]
    public class Application
    {
        // Make this lazy so you don't build it out
        // when you don't need it.
        private static readonly Lazy<IAlbaHost> host = new Lazy<IAlbaHost>(() => Create());

        public static IAlbaHost Host => host.Value;

        // Make sure that NUnit will shut down the AlbaHost when
        // all the projects are finished
        [OneTimeTearDown]
        public void Teardown()
        {
            if (host.IsValueCreated)
            {
                host.Value.Dispose();
            }
        }

        private static IAlbaHost Create() => AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddMvcCore();
            });
        }).GetAwaiter().GetResult();
    }
}
