using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json.Serialization;
using EMCR.DRR.Managers.Intake;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class DRIFApplicationController : ControllerBase
    {
        private readonly ILogger<DRIFApplicationController> logger;
        private readonly IIntakeManager intakeManager;

        public DRIFApplicationController(ILogger<DRIFApplicationController> logger, IIntakeManager intakeManager)
        {
            this.logger = logger;
            this.intakeManager = intakeManager;
        }


        [HttpPost("EOI")]
        public async Task<ActionResult<ApplicationResult>> CreateEOIApplication(DrifEoiApplication application)
        {
            var id = await intakeManager.Handle(new DrifEoiApplicationCommand { application = application });
            return Ok(new ApplicationResult { Id = id });
        }
    }

    public static class ApplicationValidators
    {
        public const int CONTACT_MAX_LENGTH = 40;
        public const int ACCOUNT_MAX_LENGTH = 100;
        public const double FUNDING_MAX_VAL = 999999999.99;
    }

    public class ApplicationResult
    {
        public required string Id { get; set; }
    }

    public class DrifEoiApplication
    {
        //Proponent Information
        public ProponentType ProponentType { get; set; }
        public required string ProponentName { get; set; }
        public required ContactDetails Submitter { get; set; }
        public required ContactDetails ProjectContact { get; set; }
        public required IEnumerable<ContactDetails> AdditionalContacts { get; set; }
        [CollectionStringLengthValid(ErrorMessage = "PartneringProponents have a limit of 40 characters per name")]
        public required IEnumerable<string> PartneringProponents { get; set; }

        //Project Information
        public required FundingStream FundingStream { get; set; }
        public required string ProjectTitle { get; set; }
        public ProjectType ProjectType { get; set; }
        public required string ScopeStatement { get; set; }
        public required IEnumerable<Hazards> RelatedHazards { get; set; }
        public string? OtherHazardsDescription { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }

        //Funding Information
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public required decimal EstimatedTotal { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public required decimal FundingRequest { get; set; }
        public required IEnumerable<FundingInformation> OtherFunding { get; set; }
        public required decimal RemainingAmount { get; set; }
        public string? IntendToSecureFunding { get; set; }

        //Location Information
        public required bool OwnershipDeclaration { get; set; }
        public string? OwnershipDescription { get; set; }
        public required string LocationDescription { get; set; }

        //Project Detail
        public required string RationaleForFunding { get; set; }
        public required EstimatedNumberOfPeople EstimatedPeopleImpacted { get; set; }
        public required string CommunityImpact { get; set; }
        public required IEnumerable<string> InfrastructureImpacted { get; set; }
        public required string DisasterRiskUnderstanding { get; set; }
        public string? AdditionalBackgroundInformation { get; set; }
        public required string AddressRisksAndHazards { get; set; }
        public required string DRIFProgramGoalAlignment { get; set; }
        public string? AdditionalSolutionInformation { get; set; }
        public required string RationaleForSolution { get; set; }

        //Engagement Plan
        public required string FirstNationsEngagement { get; set; }
        public required string NeighbourEngagement { get; set; }
        public string? AdditionalEngagementInformation { get; set; }

        //Other Supporting Information
        public required string ClimateAdaptation { get; set; }
        public string? OtherInformation { get; set; }


        //Declaration
        public required bool IdentityConfirmation { get; set; }
        public bool? FOIPPAConfirmation { get; set; }
        public required bool FinancialAwarenessConfirmation { get; set; }
    }

    public class FundingInformation
    {
        public required string Name { get; set; }
        public required FundingType Type { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public required decimal Amount { get; set; }
        public string? OtherDescription { get; set; }

    }

    public class ContactDetails
    {
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public required string FirstName { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public required string LastName { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public required string Title { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public required string Department { get; set; }
        //[RegularExpression("^\\d\\d\\d-\\d\\d\\d-\\d\\d\\d\\d$", ErrorMessage = "Phone number must be of the format '000-000-0000'")]
        public required string Phone { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public required string Email { get; set; }

    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProponentType
    {
        [Description("First Nation")]
        FirstNation,

        [Description("Local Government")]
        LocalGovernment,

        [Description("Regional District")]
        RegionalDistrict
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FundingStream
    {
        [Description("Foundational and Non-Structural")]
        Stream1,

        [Description("Structural")]
        Stream2
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EstimatedNumberOfPeople
    {
        [Description("1 - 10,000")]
        OneToTenK,

        [Description("10,001 - 50,000")]
        TenKToFiftyK,

        [Description("50,001 - 100k")]
        FiftyKToHundredK,

        [Description("100,001 +")]
        HundredKPlus,

        [Description("Unsure")]
        Unsure,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProjectType
    {
        [Description("New Project")]
        New,

        [Description("New Phase of Existing Project")]
        Existing
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FundingType
    {
        [Description("Federal")]
        Fed,

        [Description("Federal/Provincial")]
        FedProv,

        [Description("Provincial")]
        Prov,

        [Description("Self-funded")]
        SelfFunding,

        [Description("Other Grants")]
        OtherGrants,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Hazards
    {
        [Description("Drought and water scarcity")]
        Drought,

        [Description("Extreme Temperature")]
        ExtremeTemperature,

        [Description("Flood")]
        Flood,

        [Description("Geohazards (e.g., avalanche, landslide)")]
        Geohazards,

        [Description("Sea Level Rise")]
        SeaLevelRise,

        [Description("Seismic")]
        Seismic,

        [Description("Tsunami")]
        Tsunami,

        [Description("Other")]
        Other,
    }

#pragma warning disable CS8765 // nullability
    public class CollectionStringLengthValid : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (!(value is IList)) return false;
            foreach (string item in (IList)value)
            {
                if (item.Length > ApplicationValidators.ACCOUNT_MAX_LENGTH) return false;
            }
            return true;
        }
    }
}
#pragma warning restore CS8765
