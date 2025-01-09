using AutoMapper;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using EMCR.Utilities.Extensions;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Projects
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IDRRContextFactory dRRContextFactory;
        private readonly IMapper mapper;

        public ProjectRepository(IDRRContextFactory dRRContextFactory, IMapper mapper)
        {
            this.mapper = mapper;
            this.dRRContextFactory = dRRContextFactory;
        }

        public async Task<ProjectQueryResult> Query(ProjectQuery query)
        {
            return query switch
            {
                ProjectsQuery q => await HandleQueryProject(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        private async Task<ProjectQueryResult> HandleQueryProject(ProjectsQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var projectsQuery = readCtx.drr_projects
                .Where(a => a.statuscode != (int)ProjectStatusOptionSet.Inactive);
            if (!string.IsNullOrEmpty(query.Id)) projectsQuery = projectsQuery.Where(a => a.drr_name == query.Id);

            var results = (await projectsQuery.GetAllPagesAsync(ct)).ToList();
            var length = results.Count;

            //results = SortAndPageResults(results, query);

            await Parallel.ForEachAsync(results, ct, async (prj, ct) => await ParallelLoadProjectAsync(readCtx, prj, ct));
            return new ProjectQueryResult { Items = mapper.Map<IEnumerable<Project>>(results), Length = length };
        }

        private static async Task ParallelLoadProjectAsync(DRRContext ctx, drr_project project, CancellationToken ct)
        {
            ctx.AttachTo(nameof(DRRContext.drr_projects), project);

            var loadTasks = new List<Task>
            {
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_FullProposalApplication), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_Case), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_Program), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_ReportingSchedule), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_drr_project_drr_projectreport_Project), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_drr_project_drr_projectclaim_Project), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_drr_project_drr_projectprogress_Project), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_drr_project_drr_projectbudgetforecast_Project), ct),
                ctx.LoadPropertyAsync(project, nameof(drr_project.drr_drr_project_drr_projectevent_Project), ct),
            };

            await Task.WhenAll(loadTasks);

            //await ctx.LoadPropertyAsync(project, nameof(drr_project.drr_Case.drr_EOIApplication), ct);

            //project.drr_application_fundingsource_Application = new System.Collections.ObjectModel.Collection<drr_fundingsource>(project.drr_application_fundingsource_Application.Where(c => c.statecode == (int)EntityState.Active).ToList());

            await Task.WhenAll([
                ParallelLoadWorkplanActivities(ctx, project, ct),
                ]);
        }

        private static async Task ParallelLoadWorkplanActivities(DRRContext ctx, drr_project project, CancellationToken ct)
        {
            await project.drr_drr_project_drr_projectprogress_Project.ForEachAsync(5, async report =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_projectprogresses), report);
                await ctx.LoadPropertyAsync(report, nameof(drr_projectprogress.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport), ct);
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
