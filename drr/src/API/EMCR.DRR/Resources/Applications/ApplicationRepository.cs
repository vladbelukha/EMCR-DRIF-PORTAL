using System.Collections.ObjectModel;
using AutoMapper;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
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
                SubmitApplication c => await HandleSubmitEOIApplication(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ApplicationQueryResult> Query(ApplicationsQuery query)
        {
            return query switch
            {
                ApplicationsQuery q => await HandleQueryApplication(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        private async Task<ApplicationQueryResult> HandleQueryApplication(ApplicationsQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var applicationsQuery = readCtx.drr_applications.Where(a => a.statecode == (int)EntityState.Active);
            if (!string.IsNullOrEmpty(query.ApplicationId)) applicationsQuery = applicationsQuery.Where(f => f.drr_applicationid == Guid.Parse(query.ApplicationId));

            var results = await applicationsQuery.GetAllPagesAsync(ct);

            results = results.ToArray();

            await Parallel.ForEachAsync(results, ct, async (f, ct) => await ParallelLoadApplicationAsync(readCtx, f, ct));
            var items = mapper.Map<IEnumerable<Application>>(results);
            return new ApplicationQueryResult { Items = items };
        }

        public async Task<ManageApplicationCommandResult> HandleSubmitEOIApplication(SubmitApplication cmd)
        {
            var ctx = dRRContextFactory.Create();
            var drrApplication = mapper.Map<drr_application>(cmd.Application);
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

        private static async Task ParallelLoadApplicationAsync(DRRContext ctx, drr_application application, CancellationToken ct)
        {
            ctx.AttachTo(nameof(DRRContext.drr_applications), application);

            var loadTasks = new List<Task>
            {
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_SubmitterContactInformation), ct),
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_application_contact_Application), ct),
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_application_fundingsource_Application), ct)
            };

            await Task.WhenAll(loadTasks);
        }
    }
}
