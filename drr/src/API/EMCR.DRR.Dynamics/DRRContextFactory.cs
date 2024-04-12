using EMCR.DRR.Dynamics;
using Microsoft.Extensions.Options;
using Microsoft.OData.Client;
using Microsoft.OData.Extensions.Client;

namespace EMCR.DRR.Dynamics
{
    public interface IDRRContextFactory
    {
        DRRContext Create();

        DRRContext CreateReadOnly();
    }

    internal class DRRContextFactory : IDRRContextFactory
    {
        private readonly IODataClientFactory odataClientFactory;
        private readonly DRRContextOptions dynamicsOptions;

        public DRRContextFactory(IODataClientFactory odataClientFactory, IOptions<DRRContextOptions> dynamicsOptions)
        {
            this.odataClientFactory = odataClientFactory;
            this.dynamicsOptions = dynamicsOptions.Value;
        }

        public DRRContext Create() => Create(MergeOption.AppendOnly);

        public DRRContext CreateReadOnly() => Create(MergeOption.NoTracking);

        private DRRContext Create(MergeOption mergeOption)
        {
            var ctx = odataClientFactory.CreateClient<DRRContext>(dynamicsOptions.DynamicsApiBaseUri, "DRR_dynamics");
            ctx.MergeOption = mergeOption;
            return ctx;
        }
    }
}
