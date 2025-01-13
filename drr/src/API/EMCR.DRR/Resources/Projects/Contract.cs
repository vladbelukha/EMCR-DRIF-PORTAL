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

    public enum ProjectReportStatusOptionSet
    {
        InReview = 1,
        Approved = 172580000,
        Rejected = 172580001,
        Skipped = 172580002,
        Inactive = 2,
    }

    public enum ProjectClaimStatusOptionSet
    {
        Approved = 1,
        Rejected = 172580000,
        Invalid = 172580001,
        InProgress = 172580002,
        Inactive = 2,
    }

    public enum ProjectProgressReportStatusOptionSet
    {
        Draft = 1,
        Inactive = 2,
    }

    public enum ForecastStatusOptionSet
    {
        NotStarted = 1,
        Draft = 172580000,
        Submitted = 172580001,
        UpdateNeeded = 1172580002,
        Approved = 172580003,
        Inactive = 2,
    }

    public enum PeriodTypeOptionSet
    {
        Periodical = 172580000,
        Final = 172580001,
        Interim = 172580002,
    }

    public enum WorkplanProgressOptionSet
    {
        NotStarted = 172580000,
        InProgress = 172580001,
        Completed = 172580002,
        NotApplicable = 172580003
    }

    public enum EventTypeOptionSet
    {
        GroundBreaking = 172580000,
        RibbonCuttingOpening = 172580001,
        CommunityEngagement = 172580002,
        Other = 172580003,
    }

    public enum EventStatusOptionSet
    {
        NotPlanned = 172580000,
        PlannedDateUnknown = 172580001,
        PlannedDateKnown= 172580002,
        AlreadyOccurred= 172580003,
        Unknown= 172580004,
    }
}
