using AutoMapper;
using EMCR.DRR.Dynamics;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.Resources.Applications
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IDRRContextFactory dRRContextFactory;
        private readonly IMapper mapper;

        public ApplicationRepository(IDRRContextFactory dRRContextFactory, IMapper mapper)
        {
            this.mapper = mapper;
            this.dRRContextFactory = dRRContextFactory;
        }

        public async Task<ManageApplicationCommandResult> Manage(ManageApplicationCommand cmd)
        {
            return cmd switch
            {
                SubmitEOIApplication c => await HandleSubmitEOIApplication(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ManageApplicationCommandResult> HandleSubmitEOIApplication(SubmitEOIApplication cmd)
        {
            var ctx = dRRContextFactory.Create();
            var drifApplication = mapper.Map<Drr_drifapplication>(cmd.EOIApplication);
            drifApplication.Drr_drifapplicationid = Guid.NewGuid();
            ctx.AddToDrr_drifapplications(drifApplication);
            await ctx.SaveChangesAsync();
            ctx.DetachAll();
            return new ManageApplicationCommandResult { Id = drifApplication.Drr_drifapplicationid?.ToString() ?? string.Empty };
        }
    }
}
