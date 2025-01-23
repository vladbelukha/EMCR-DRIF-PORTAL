using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EMCR.DRR.API.Services;
using EMCR.DRR.Managers.Intake;
using System.Security.Claims;
using System.ComponentModel;
using System.Text.Json.Serialization;
using EMCR.DRR.API.Model;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> logger;
        private readonly IIntakeManager intakeManager;
        private readonly IMapper mapper;
        private readonly ErrorParser errorParser;

#pragma warning disable CS8603 // Possible null reference return.
        private string GetCurrentBusinessId() => User.FindFirstValue("bceid_business_guid");
        private string GetCurrentBusinessName() => User.FindFirstValue("bceid_business_name");
        private string GetCurrentUserId() => User.FindFirstValue("bceid_user_guid");
        private UserInfo GetCurrentUser()
        {
            return new UserInfo { BusinessId = GetCurrentBusinessId(), BusinessName = GetCurrentBusinessName(), UserId = GetCurrentUserId() };
        }
#pragma warning restore CS8603 // Possible null reference return.

        public ProjectController(ILogger<ProjectController> logger, IIntakeManager intakeManager, IMapper mapper)
        {
            this.logger = logger;
            this.intakeManager = intakeManager;
            this.mapper = mapper;
            this.errorParser = new ErrorParser();
        }

        [HttpGet]
        //[FromQuery] QueryOptions? options
        public async Task<ActionResult<ProjectResponse>> Get()
        {
            try
            {
                //QueryOptions = options
                var res = await intakeManager.Handle(new DrrProjectsQuery { BusinessId = GetCurrentBusinessId() });
                return Ok(new ProjectResponse { Projects = mapper.Map<IEnumerable<DraftDrrProject>>(res.Items), Length = res.Length });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DraftDrrProject>> GetProject(string id)
        {
            try
            {
                var project = (await intakeManager.Handle(new DrrProjectsQuery { Id = id, BusinessId = GetCurrentBusinessId() })).Items.FirstOrDefault();
                if (project == null) return new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = "" });
                return Ok(mapper.Map<DraftDrrProject>(project));
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<ProjectResult>> UpdateProject([FromBody] DraftDrrProject project, string id)
        {
            try
            {
                project.Id = id;

                var drr_id = await intakeManager.Handle(new SaveProjectCommand { Project = mapper.Map<DrrProject>(project), UserInfo = GetCurrentUser() });
                return Ok(new ProjectResult { Id = drr_id });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<ProjectResult>> SubmitProject([FromBody] DrrProject project, string id)
        {
            try
            {
                project.Id = id;

                var drr_id = await intakeManager.Handle(new SubmitProjectCommand { Project = project, UserInfo = GetCurrentUser() });
                return Ok(new ProjectResult { Id = drr_id });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpGet("{projectId}/interim-reports/{reportId}")]
        public async Task<ActionResult<InterimReport>> GetInterimReport(string projectId, string reportId)
        {
            try
            {
                var report = (await intakeManager.Handle(new DrrReportsQuery { Id = reportId, BusinessId = GetCurrentBusinessId() })).Items.FirstOrDefault();
                if (report == null) return new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = "" });
                return Ok(mapper.Map<InterimReport>(report));
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpGet("{projectId}/interim-reports/{reportId}/claims/{claimId}")]
        public async Task<ActionResult<ProjectClaim>> GetClaim(string projectId, string reportId, string claimId)
        {
            try
            {
                var claim = (await intakeManager.Handle(new DrrClaimsQuery { Id = claimId, BusinessId = GetCurrentBusinessId() })).Items.FirstOrDefault();
                if (claim == null) return new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = "" });
                return Ok(mapper.Map<ProjectClaim>(claim));
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpGet("{projectId}/interim-reports/{reportId}/progress-reports/{progressId}")]
        public async Task<ActionResult<ProgressReport>> GetProgressReport(string projectId, string reportId, string progressId)
        {
            try
            {
                var pr = (await intakeManager.Handle(new DrrProgressReportsQuery { Id = progressId, BusinessId = GetCurrentBusinessId() })).Items.FirstOrDefault();
                if (pr == null) return new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = "" });
                return Ok(mapper.Map<ProgressReport>(pr));
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpGet("{projectId}/interim-reports/{reportId}/forecasts/{forecastId}")]
        public async Task<ActionResult<Forecast>> GetForecastReport(string projectId, string reportId, string forecastId)
        {
            try
            {
                var forecast = (await intakeManager.Handle(new DrrForecastsQuery { Id = forecastId, BusinessId = GetCurrentBusinessId() })).Items.FirstOrDefault();
                if (forecast == null) return new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = "" });
                return Ok(mapper.Map<Forecast>(forecast));
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }
    }

    public class DraftDrrProject : DrrProject
    {

    }

    public class DrrProject
    {
        public string? Id { get; set; }
        public string? EoiId { get; set; }
        public string? FpId { get; set; }
        public string? ProjectTitle { get; set; }
        public string? ContractNumber { get; set; }
        public string? ProponentName { get; set; }
        public FundingStream? FundingStream { get; set; }
        public string? ProjectNumber { get; set; }
        public ProgramType? ProgramType { get; set; }
        public ReportingScheduleType? ReportingScheduleType { get; set; }
        public decimal? FundingAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ProjectStatus? Status { get; set; }
        public PaymentCondition[]? Conditions { get; set; }
        public ContactDetails[]? Contacts { get; set; }
        public InterimReport[]? InterimReports { get; set; }
        public ProjectClaim[]? Claims { get; set; }
        public ProgressReport[]? ProgressReports { get; set; }
        public Forecast[]? Forecast { get; set; }
        public ProjectEvent[]? Events { get; set; }
        public Attachment[]? Attachments { get; set; }
    }

    public class PaymentCondition
    {
        public string? Id { get; set; }
        public string? ConditionName { get; set; }
        public decimal? Limit { get; set; }
        public bool? ConditionMet { get; set; }
        public DateTime? DateMet { get; set; }
    }

    public class InterimReport
    {
        public string? Id { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }
        public InterimReportStatus? Status { get; set; }
        public InterimProjectType? ProjectType { get; set; }
        public PeriodType? PeriodType { get; set; }
        public ProjectClaim? ProjectClaim { get; set; }
        public ProgressReport? ProgressReport { get; set; }
        public Forecast? Forecast { get; set; }
    }

    public class ProjectClaim
    {
        public string? Id { get; set; }
        public string? ClaimType { get; set; }
        public DateTime? ClaimDate { get; set; }
        public decimal? ClaimAmount { get; set; }
        public ClaimStatus? Status { get; set; }
    }

    public class ProgressReport
    {
        public string? Id { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public DateTime? DateApproved { get; set; }
        public DateTime? DueDate { get; set; }
        public WorkPlan? WorkPlan { get; set; }
        public EventInformation? EventInformation { get; set; }
        public ProgressReportStatus? Status { get; set; }
    }

    public class EventInformation
    {
        public ProjectEvent[]? Events { get; set; }
        public bool? HaveEventsOccurred { get; set; }
    }

    public class ProjectEvent
    {
        public EventType? EventType { get; set; }
        public EventStatus? Status { get; set; }
        public DateTime? PlannedEventDate { get; set; }
        public DateTime? ActualEventDate { get; set; }
        public string? NextEventDescription { get; set; }
        public ContactDetails? EventContact { get; set; }
        public bool? ProvincialRepresentativeRequest { get; set; }
        public string? ProvincialRepresentativeRequestComment { get; set; }
    }

    public class WorkPlan
    {
        public WorkplanActivity[]? WorkplanActivities { get; set; }
        public decimal? ProjectCompletionPercentage { get; set; }
        public YesNoOption? CommunityMedia { get; set; }
        public DateTime? CommunityMediaDate { get; set; }
        public string? CommunityMediaComment { get; set; }
        public ProvincialMedia? ProvincialMedia { get; set; }
        public DateTime? ProvincialMediaDate { get; set; }
        public string? ProvincialMediaComment { get; set; }
        public string? WorksCompleted { get; set; }
        public string? OutstandingIssues { get; set; }
        public bool? FundingSourcesChanged { get; set; }
        public string? FundingSourcesChangedComment { get; set; }
    }

    public class WorkplanActivity
    {
        public ActivityType? Activity { get; set; }
        public bool? PreCreatedActivity { get; set; }
        public string? Comment { get; set; }
        public WorkplanStatus? Status { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedCompletionDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
    }

    public class Forecast
    {
        public string? Id { get; set; }
        public string? ForecastType { get; set; }
        public DateTime? ForecastDate { get; set; }
        public decimal? ForecastAmount { get; set; }
        public ForecastStatus? Status { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActivityType
    {
        [Description("Administration (up to 10%)")]
        Administration,

        [Description("Approvals/Permitting")]
        ApprovalsPermitting,

        [Description("Assessment")]
        Assessment,

        [Description("Communications")]
        Communications,

        [Description("Construction")]
        Construction,

        [Description("Construction Contract Award")]
        ConstructionContractAward,

        [Description("Construction Tender")]
        ConstructionTender,

        [Description("Design")]
        Design,

        [Description("First Nations Engagement")]
        FirstNationsEngagement,

        [Description("Land Acquisition/Property Purchase")]
        LandAcquisition,

        [Description("Mapping")]
        Mapping,

        [Description("Neighbouring jurisdictions and other impacted or affected parties engagement")]
        AffectedPartiesEngagement,

        [Description("Permit to Construct")]
        PermitToConstruct,

        [Description("Project")]
        Project,

        [Description("Project Planning")]
        ProjectPlanning,

        [Description("Proponent community(ies) engagement and public education")]
        CommunityEngagement,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProjectStatus
    {
        [Description("Active")]
        Active,

        [Description("Inactive")]
        Inactive
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ReportingScheduleType
    {
        [Description("Quarterly")]
        Quarterly,

        [Description("Monthly")]
        Monthly
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentConditionStatus
    {
        [Description("Met")]
        Met,

        [Description("NotMet")]
        NotMet
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProvincialMedia
    {
        [Description("Not Announced")]
        NotAnnounced,

        [Description("Not Applicable")]
        NotApplicable
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WorkplanStatus
    {
        [Description("Not Started")]
        NotStarted,

        [Description("In Progress")]
        InProgress,

        [Description("Completed")]
        Completed,

        [Description("Not Applicable")]
        NotApplicable
    }

    public enum EventType
    {
        [Description("Ground Breaking")]
        GroundBreaking,

        [Description("Ribbon Cutting/Opening")]
        RibbonCuttingOpening,

        [Description("Community Engagement")]
        CommunityEngagement,

        [Description("Other")]
        Other,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EventStatus
    {
        [Description("Not Planned")]
        NotPlanned,

        [Description("Planned, Date Unknown")]
        PlannedDateUnknown,

        [Description("Planned, Date Known")]
        PlannedDateKnown,

        [Description("Already Occurred")]
        AlreadyOccurred,

        [Description("Unknown")]
        Unknown,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InterimReportStatus
    {
        [Description("Not Started")]
        NotStarted,

        [Description("In Progress")]
        InProgress,

        [Description("Approved")]
        Approved,

        [Description("Skipped")]
        Skipped,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InterimProjectType
    {
        [Description("Foundational and Non-Structural")]
        Stream1,

        [Description("Structural")]
        Stream2
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PeriodType
    {
        [Description("Periodical")]
        Periodical,

        [Description("Final")]
        Final,

        [Description("Interim")]
        Interim,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClaimStatus
    {
        [Description("Approved")]
        Approved,

        [Description("Rejected")]
        Rejected,

        [Description("Invalid")]
        Invalid,

        [Description("In Progress")]
        InProgress,

        [Description("Submitted")]
        Submitted,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProgressReportStatus
    {
        [Description("NotStarted")]
        NotStarted,

        [Description("Draft")]
        Draft,

        [Description("Submitted")]
        Submitted,

        [Description("Update Needed")]
        UpdateNeeded,

        [Description("Approved")]
        Approved,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ForecastStatus
    {
        [Description("Pending")]
        Pending,

        [Description("Review")]
        Review,

        [Description("Approved")]
        Approved,

        [Description("Rejected")]
        Rejected,
    }

    public class ProjectResult
    {
        public required string Id { get; set; }
    }

    public class ProjectResponse
    {
        public IEnumerable<DraftDrrProject> Projects { get; set; } = Array.Empty<DraftDrrProject>();
        public int Length { get; set; }
    }
}
