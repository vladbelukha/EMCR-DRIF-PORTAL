using AutoMapper;
using EMCR.DRR.Dynamics;
using Microsoft.Dynamics.CRM;
using static System.Net.Mime.MediaTypeNames;

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
            var drrApplication = mapper.Map<drr_application>(cmd.EOIApplication);
            drrApplication.drr_applicationid = Guid.NewGuid();
            ctx.AddTodrr_applications(drrApplication);
            var primaryApplicant = drrApplication.drr_PrimaryApplicant;
            AssignPrimaryApplicant(ctx, drrApplication, primaryApplicant);
            var submitter = drrApplication.drr_SubmitterContactInformation;
            AssignSubmitter(ctx, drrApplication, submitter);
            AddProjectContacts(ctx, drrApplication);
            AddFundinSources(ctx, drrApplication);

            await ctx.SaveChangesAsync();
            ctx.DetachAll();
            return new ManageApplicationCommandResult { Id = drrApplication.drr_applicationid?.ToString() ?? string.Empty };
        }

        private static void AssignPrimaryApplicant(DRRContext drrContext, drr_application application, account primaryApplicant)
        {
            drrContext.AddToaccounts(primaryApplicant);
            drrContext.AddLink(primaryApplicant, nameof(primaryApplicant.drr_account_application_PrimaryApplicant), application);
        }

        private static void AssignSubmitter(DRRContext drrContext, drr_application application, contact submitter)
        {
            drrContext.AddTocontacts(submitter);
            //drrContext.AddLink(application, nameof(application.drr_contact_application_SubmitterContactInformation), submitter);
            drrContext.AddLink(submitter, nameof(submitter.drr_contact_application_SubmitterContactInformation), application);
        }

        private static void AddProjectContacts(DRRContext drrContext, drr_application application)
        {
            foreach (var projectContact in application.drr_application_contact_Application)
            {
                drrContext.AddTocontacts(projectContact);
                drrContext.AddLink(application, nameof(application.drr_application_contact_Application), projectContact);
                drrContext.SetLink(projectContact, nameof(projectContact.drr_Application), application);
            }
        }

        private static void AddFundinSources(DRRContext drrContext, drr_application application)
        {
            foreach (var fund in application.drr_application_fundingsource_Application)
            {
                drrContext.AddTodrr_fundingsources(fund);
                drrContext.AddLink(application, nameof(application.drr_application_fundingsource_Application), fund);
                drrContext.SetLink(fund, nameof(fund.drr_Application), application);
            }
        }
    }
}
