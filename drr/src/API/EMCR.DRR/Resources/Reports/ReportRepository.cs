using AutoMapper;
using EMCR.DRR.API.Resources.Projects;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using EMCR.Utilities.Extensions;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Reports
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDRRContextFactory dRRContextFactory;
        private readonly IMapper mapper;

        public ReportRepository(IDRRContextFactory dRRContextFactory, IMapper mapper)
        {
            this.mapper = mapper;
            this.dRRContextFactory = dRRContextFactory;
        }

        public async Task<ReportQueryResult> Query(ReportQuery query)
        {
            return query switch
            {
                ReportsQuery q => await HandleQueryReport(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        public async Task<ClaimQueryResult> Query(ClaimQuery query)
        {
            return query switch
            {
                ClaimsQuery q => await HandleQueryClaim(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        public async Task<ProgressReportQueryResult> Query(ProgressReportQuery query)
        {
            return query switch
            {
                ProgressReportsQuery q => await HandleQueryProgressReport(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        public async Task<ForecastQueryResult> Query(ForecastQuery query)
        {
            return query switch
            {
                ForecastsQuery q => await HandleQueryForecast(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        private async Task<ReportQueryResult> HandleQueryReport(ReportsQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var claimsQuery = readCtx.drr_projectreports
                .Where(a => a.statuscode != (int)ProjectReportStatusOptionSet.Inactive);
            if (!string.IsNullOrEmpty(query.Id)) claimsQuery = claimsQuery.Where(a => a.drr_name == query.Id);

            var results = (await claimsQuery.GetAllPagesAsync(ct)).ToList();
            var length = results.Count;

            await Parallel.ForEachAsync(results, ct, async (rep, ct) => await ParallelLoadReportDetails(readCtx, rep, ct));

            return new ReportQueryResult { Items = mapper.Map<IEnumerable<InterimReportDetails>>(results), Length = length };
        }

        private async Task<ClaimQueryResult> HandleQueryClaim(ClaimsQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var claimsQuery = readCtx.drr_projectclaims
                .Where(a => a.statuscode != (int)ProjectClaimStatusOptionSet.Inactive);
            if (!string.IsNullOrEmpty(query.Id)) claimsQuery = claimsQuery.Where(a => a.drr_name == query.Id);

            var results = (await claimsQuery.GetAllPagesAsync(ct)).ToList();
            var length = results.Count;

            return new ClaimQueryResult { Items = mapper.Map<IEnumerable<ClaimDetails>>(results), Length = length };
        }

        private async Task<ProgressReportQueryResult> HandleQueryProgressReport(ProgressReportsQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var progressReportsQuery = readCtx.drr_projectprogresses
                .Where(a => a.statuscode != (int)ProjectProgressReportStatusOptionSet.Inactive);
            if (!string.IsNullOrEmpty(query.Id)) progressReportsQuery = progressReportsQuery.Where(a => a.drr_name == query.Id);

            var results = (await progressReportsQuery.GetAllPagesAsync(ct)).ToList();
            var length = results.Count;

            await Parallel.ForEachAsync(results, ct, async (pr, ct) => await ParallelLoadWorkplanActivities(readCtx, pr, ct));
            return new ProgressReportQueryResult { Items = mapper.Map<IEnumerable<ProgressReportDetails>>(results), Length = length };
        }

        private async Task<ForecastQueryResult> HandleQueryForecast(ForecastsQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var forecastsQuery = readCtx.drr_projectbudgetforecasts
                .Where(a => a.statuscode != (int)ProjectStatusOptionSet.Inactive);
            if (!string.IsNullOrEmpty(query.Id)) forecastsQuery = forecastsQuery.Where(a => a.drr_name == query.Id);

            var results = (await forecastsQuery.GetAllPagesAsync(ct)).ToList();
            var length = results.Count;

            return new ForecastQueryResult { Items = mapper.Map<IEnumerable<ForecastDetails>>(results), Length = length };
        }

        private static async Task ParallelLoadReportDetails(DRRContext ctx, drr_projectreport report, CancellationToken ct)
        {
            ctx.AttachTo(nameof(DRRContext.drr_projectreports), report);
            var loadTasks = new List<Task>
            {
                ctx.LoadPropertyAsync(report, nameof(drr_projectreport.drr_ClaimReport), ct),
                ctx.LoadPropertyAsync(report, nameof(drr_projectreport.drr_ProgressReport), ct),
                ctx.LoadPropertyAsync(report, nameof(drr_projectreport.drr_BudgetForecast), ct),
            };

            await Task.WhenAll(loadTasks);
        }

        private static async Task ParallelLoadWorkplanActivities(DRRContext ctx, drr_projectprogress pr, CancellationToken ct)
        {
            ctx.AttachTo(nameof(DRRContext.drr_projectprogresses), pr);
            await ctx.LoadPropertyAsync(pr, nameof(drr_projectprogress.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport), ct);
            await ParallelLoadActivityTypes(ctx, pr, ct);
        }

        private static async Task ParallelLoadActivityTypes(DRRContext ctx, drr_projectprogress pr, CancellationToken ct)
        {
            await pr.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport.ForEachAsync(5, async wa =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_projectworkplanactivities), wa);
                await ctx.LoadPropertyAsync(wa, nameof(drr_projectworkplanactivity.drr_ActivityType), ct);
            });
        }

        public async Task<bool> CanAccessProject(string id, string businessId)
        {
            var readCtx = dRRContextFactory.CreateReadOnly();
            var existingProject = await readCtx.drr_projects.Expand(a => a.drr_ProponentName).Where(a => a.drr_name == id).SingleOrDefaultAsync();
            if (existingProject == null) return true;
            return (!string.IsNullOrEmpty(existingProject.drr_ProponentName.drr_bceidguid)) && existingProject.drr_ProponentName.drr_bceidguid.Equals(businessId);
        }
    }
}
