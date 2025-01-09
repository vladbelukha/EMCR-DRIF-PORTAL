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
        public PaymentConditionStatus? Status { get; set; }
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
        public ProjectClaim? Claim { get; set; }
        public ProgressReport? Report { get; set; }
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
        public string? ReportType { get; set; }
        public DateTime? ReportDate { get; set; }
        public WorkplanActivity[]? WorkplanActivities { get; set; }
        public ProgressReportStatus? Status { get; set; }
    }

    public class ProjectEvent
    {
        public EventStatus? status { get; set; }
    }

    public class WorkplanActivity
    {
        public WorkplanProgress? Progress { get; set; }
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
    public enum WorkplanProgress
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
        [Description("In Review")]
        InReview,

        [Description("Approved")]
        Approved,

        [Description("Rejected")]
        Rejected,

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
    public enum ProgressReportStatus
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
