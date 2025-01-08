using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;

namespace EMCR.DRR.API.Resources.Projects
{
    public interface IProjectRepository
    {
        Task<ProjectQueryResult> Query(ProjectQuery query);
        Task<bool> CanAccessProject(string id, string businessId);
    }

    public abstract class ProjectQuery
    { }

    public class ProjectQueryResult
    {
        public IEnumerable<Project> Items { get; set; } = Array.Empty<Project>();
        public int Length { get; set; }
    }

    public class ProjectsQuery : ProjectQuery
    {
        public string? Id { get; set; }
        public string? BusinessId { get; set; }
        public int Page { get; set; } = 0;
        public int Count { get; set; } = 0;
        public string? OrderBy { get; set; }
        public FilterOptions? FilterOptions { get; set; }
    }

    //public class ProjectFilterOptions
    //{
    //    public string? ProejctType { get; set; }
    //    public string? ProgramType { get; set; }
    //    public List<int>? Statuses { get; set; }
    //}

    public enum ProjectStatusOptionSet
    {
        InProgress = 1,
        Completed = 172580000,
        Inactive = 2,
    }
}
