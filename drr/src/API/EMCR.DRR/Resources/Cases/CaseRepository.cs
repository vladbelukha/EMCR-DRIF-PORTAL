using AutoMapper;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Cases
{
    public class CaseRepository : ICaseRepository
    {
        private readonly IDRRContextFactory dRRContextFactory;
        private readonly IMapper mapper;

        public CaseRepository(IDRRContextFactory dRRContextFactory, IMapper mapper)
        {
            this.dRRContextFactory = dRRContextFactory;
            this.mapper = mapper;
        }

        public async Task<ManageCaseCommandResult> Manage(ManageCaseCommand cmd)
        {
            return cmd switch
            {
                GenerateFpFromEoi c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<QueryCaseCommandResult> Query(QueryCaseCommand query)
        {
            return query switch
            {
                CaseQuery q => await HandleCaseQuery(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        private async Task<QueryCaseCommandResult> HandleCaseQuery(CaseQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var caseQuery = readCtx.incidents.Expand(i => i.drr_EOIApplication).Expand(i => i.drr_FullProposalApplication).Where(a => a.statecode == (int)EntityState.Active);
            if (!string.IsNullOrEmpty(query.EoiId)) caseQuery = caseQuery.Where(i => i.drr_EOIApplication.drr_name == query.EoiId);
            if (!string.IsNullOrEmpty(query.FpId)) caseQuery = caseQuery.Where(i => i.drr_FullProposalApplication.drr_name == query.FpId);

            var results = (await caseQuery.GetAllPagesAsync(ct)).ToArray();
            var items = mapper.Map<IEnumerable<Case>>(results);
            return new QueryCaseCommandResult { Items = items };
        }

        private async Task<ManageCaseCommandResult> Handle(GenerateFpFromEoi cmd)
        {
            var ctx = dRRContextFactory.Create();
            var incident = await ctx.incidents.Where(i => i.drr_EOIApplication.drr_name == cmd.EoiId).SingleOrDefaultAsync();
            incident.drr_createfullproposal = (int)DRRTwoOptions.Yes;
            ctx.UpdateObject(incident);
            await ctx.SaveChangesAsync();
            ctx.DetachAll();
            var updatedIncident = await ctx.incidents.Expand(i => i.drr_FullProposalApplication).Where(i => i.drr_EOIApplication.drr_name == cmd.EoiId).SingleOrDefaultAsync();

            var fp = await ctx.drr_applications.Where(a => a.drr_name == updatedIncident.drr_FullProposalApplication.drr_name).SingleOrDefaultAsync();
            if (fp != null)
            {
                if (cmd.ScreenerQuestions.ProjectWorkplan == true) fp.drr_detailedprojectworkplan = (int)DRRTwoOptions.Yes;
                if (cmd.ScreenerQuestions.ProjectSchedule == true) fp.drr_projectschedule = (int)DRRTwoOptions.Yes;
                if (cmd.ScreenerQuestions.CostEstimate == true) fp.drr_detailedcostestimate = (int)DRRTwoOptions.Yes;
                if (cmd.ScreenerQuestions.SitePlan == YesNoOption.Yes) fp.drr_siteplan = (int)DRRYesNoNotApplicable.Yes;
                if (cmd.ScreenerQuestions.HaveAuthorityToDevelop == true) fp.drr_proponenthastheauthorityandownership = (int)DRRTwoOptions.Yes;
                if (cmd.ScreenerQuestions.FirstNationsAuthorizedByPartners == YesNoOption.Yes) fp.drr_authorizedendorsedfirstnationpartners = (int)DRRYesNoNotApplicable.Yes;
                if (cmd.ScreenerQuestions.LocalGovernmentAuthorizedByPartners == YesNoOption.Yes) fp.drr_authorizedendorsedlocalgovpartners = (int)DRRYesNoNotApplicable.Yes;
                if (cmd.ScreenerQuestions.FoundationWorkCompleted == YesNoOption.Yes) fp.drr_foundationalorpreviouswork = (int)DRRYesNoNotApplicable.Yes;
                if (cmd.ScreenerQuestions.EngagedWithFirstNationsOccurred == true) fp.drr_meaningfullyengagedwithlocalfirstnations = (int)DRRTwoOptions.Yes;
                if (cmd.ScreenerQuestions.IncorporateFutureClimateConditions == true) fp.drr_doesprojectconsiderclimatechange = (int)DRRTwoOptions.Yes;
                if (cmd.ScreenerQuestions.MeetsRegulatoryRequirements == true) fp.drr_requiredagencydiscussionsandapprovals = (int)DRRTwoOptions.Yes;
                if (cmd.ScreenerQuestions.MeetsEligibilityRequirements == true) fp.drr_willprojectmeetreqsforallpermitsetc = (int)DRRTwoOptions.Yes;

                ctx.UpdateObject(fp);
                await ctx.SaveChangesAsync();
            }

            return new ManageCaseCommandResult { Id = updatedIncident.drr_FullProposalApplication.drr_name };
        }
    }
}
