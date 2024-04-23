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
            var primaryProponent = drrApplication.drr_Primary_Proponent_Name;
            AssignPrimaryProponent(ctx, drrApplication, primaryProponent);
            var submitter = drrApplication.drr_SubmitterContact;
            AssignSubmitter(ctx, drrApplication, submitter);
            var primaryProjectContact = drrApplication.drr_PrimaryProjectContact;
            AssignPrimaryProjectContact(ctx, drrApplication, primaryProjectContact);
            var additionalContact1 = drrApplication.drr_AdditionalContact1;
            var additionalContact2 = drrApplication.drr_AdditionalContact2;
            if (additionalContact1 != null && !string.IsNullOrEmpty(additionalContact1.firstname)) AddAdditionalContact1(ctx, drrApplication, additionalContact1);
            if (additionalContact2 != null && !string.IsNullOrEmpty(additionalContact2.firstname)) AddAdditionalContact2(ctx, drrApplication, additionalContact2);
            AddPartneringProponents(ctx, drrApplication);
            AddFundinSources(ctx, drrApplication);

            await ctx.SaveChangesAsync();
            ctx.DetachAll();
            return new ManageApplicationCommandResult { Id = drrApplication.drr_applicationid?.ToString() ?? string.Empty };
        }

        private static void AssignPrimaryProponent(DRRContext drrContext, drr_application application, account primaryProponent)
        {
            drrContext.AddToaccounts(primaryProponent);
            drrContext.AddLink(primaryProponent, nameof(primaryProponent.drr_account_drr_application_PrimaryProponentName), application);
        }

        private static void AssignSubmitter(DRRContext drrContext, drr_application application, contact submitter)
        {
            drrContext.AddTocontacts(submitter);
            drrContext.AddLink(submitter, nameof(submitter.drr_contact_drr_application_SubmitterContact), application);
        }

        private static void AssignPrimaryProjectContact(DRRContext drrContext, drr_application application, contact primaryProjectContact)
        {
            drrContext.AddTocontacts(primaryProjectContact);
            drrContext.AddLink(primaryProjectContact, nameof(primaryProjectContact.drr_contact_drr_application_PrimaryProjectContact), application);
        }

        private static void AddAdditionalContact1(DRRContext drrContext, drr_application application, contact additionalContact1)
        {
            drrContext.AddTocontacts(additionalContact1);
            drrContext.AddLink(additionalContact1, nameof(additionalContact1.drr_contact_drr_application_AdditionalContact1), application);
        }

        private static void AddAdditionalContact2(DRRContext drrContext, drr_application application, contact additionalContact2)
        {
            drrContext.AddTocontacts(additionalContact2);
            drrContext.AddLink(additionalContact2, nameof(additionalContact2.drr_contact_drr_application_AdditionalContact2), application);
        }

        private static void AddPartneringProponents(DRRContext drrContext, drr_application application)
        {
            foreach (var partner in application.drr_drr_application_account)
            {
                drrContext.AddToaccounts(partner);
                drrContext.AddLink(application, nameof(application.drr_drr_application_account), partner);
                drrContext.SetLink(partner, nameof(partner.drr_application_accountId), application);
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
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_SubmitterContact), ct),
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_application_contact_Application), ct),
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_application_fundingsource_Application), ct)
            };

            await Task.WhenAll(loadTasks);
        }
    }
}
