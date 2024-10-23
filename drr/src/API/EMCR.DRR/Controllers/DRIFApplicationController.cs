using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json.Serialization;
using AutoMapper;
using EMCR.DRR.API.Model;
using EMCR.DRR.API.Services;
using EMCR.DRR.Managers.Intake;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize]
    public partial class DRIFApplicationController : ControllerBase
    {
        private readonly ILogger<DRIFApplicationController> logger;
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

        public DRIFApplicationController(ILogger<DRIFApplicationController> logger, IIntakeManager intakeManager, IMapper mapper)
        {
            this.logger = logger;
            this.intakeManager = intakeManager;
            this.mapper = mapper;
            this.errorParser = new ErrorParser();
        }

        [HttpGet]
        public async Task<ActionResult<SubmissionResponse>> Get([FromQuery] QueryOptions? options)
        {
            try
            {
                var res = await intakeManager.Handle(new DrrApplicationsQuery { BusinessId = GetCurrentBusinessId(), QueryOptions = options });
                return Ok(new SubmissionResponse { Submissions = mapper.Map<IEnumerable<Submission>>(res.Items), Length = res.Length });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpGet("declarations")]
        public async Task<ActionResult<DeclarationResult>> GetDeclarations()
        {
            try
            {
                var res = await intakeManager.Handle(new DeclarationQuery());
                return Ok(new DeclarationResult { Items = mapper.Map<IEnumerable<DeclarationInfo>>(res.Items) });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        //Prevent empty additional contact 1, but populated additional contact 2
        private IEnumerable<ContactDetails> MapAdditionalContacts(DraftApplication application)
        {
            var additionalContact1 = application.AdditionalContacts.FirstOrDefault();
            var additionalContact2 = application.AdditionalContacts.ElementAtOrDefault(1);
            if (IsEmptyContact(additionalContact1))
            {
                additionalContact1 = additionalContact2;
                additionalContact2 = null;
            }
            return [additionalContact1 ?? new ContactDetails(), additionalContact2 ?? new ContactDetails()];
        }

        private bool IsEmptyContact(ContactDetails? contact)
        {
            if (contact == null) return true;
            if (string.IsNullOrEmpty(contact.FirstName)
                && string.IsNullOrEmpty(contact.LastName)
                && string.IsNullOrEmpty(contact.Title)
                && string.IsNullOrEmpty(contact.Department)
                && string.IsNullOrEmpty(contact.Phone)
                && string.IsNullOrEmpty(contact.Email)) return true;
            return false;
        }
    }

    public class DeclarationResult
    {
        public IEnumerable<DeclarationInfo> Items { get; set; } = Array.Empty<DeclarationInfo>();
    }

    public class DeclarationInfo
    {
        public required DeclarationType Type { get; set; }
        public required ApplicationType ApplicationType { get; set; }
        public required string Text { get; set; }
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

    public class QueryOptions
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; } = "Id";
        public string? Filter { get; set; } = string.Empty;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public abstract class DraftApplication
    {
        public abstract IEnumerable<ContactDetails> AdditionalContacts { get; set; }
    }

    public class DraftEoiApplication : DraftApplication
    {
        public string? FpId { get; set; }
        public string? Id { get; set; }
        public SubmissionPortalStatus? Status { get; set; }

        //Proponent Information - 1
        public ProponentType? ProponentType { get; set; }
        public ContactDetails? Submitter { get; set; }
        [Mandatory]
        public ContactDetails? ProjectContact { get; set; }
        public override IEnumerable<ContactDetails> AdditionalContacts { get; set; }
        [CollectionStringLengthValid(ErrorMessage = "PartneringProponents have a limit of 40 characters per name")]
        public IEnumerable<string> PartneringProponents { get; set; }

        //Project Information - 2
        public FundingStream? FundingStream { get; set; }
        [Mandatory]
        public string? ProjectTitle { get; set; }
        [Mandatory]
        public ProjectType? Stream { get; set; }
        public string? ScopeStatement { get; set; }
        [Mandatory]
        public IEnumerable<Hazards>? RelatedHazards { get; set; }
        public string? OtherHazardsDescription { get; set; }
        [Mandatory]
        public DateTime? StartDate { get; set; }
        [Mandatory]
        public DateTime? EndDate { get; set; }

        //Funding Information - 3
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? EstimatedTotal { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? FundingRequest { get; set; }
        [Mandatory]
        public bool? HaveOtherFunding { get; set; }
        [MandatoryIf("HaveOtherFunding", true)]
        public IEnumerable<FundingInformation> OtherFunding { get; set; }
        public decimal? RemainingAmount { get; set; }
        public string? IntendToSecureFunding { get; set; }

        //Location Information - 4
        public bool? OwnershipDeclaration { get; set; }
        [MandatoryIf("OwnershipDeclaration", false)]
        public string? OwnershipDescription { get; set; }
        [Mandatory]
        public string? LocationDescription { get; set; }

        //Project Detail - 5
        public string? RationaleForFunding { get; set; }
        [Mandatory]
        public EstimatedNumberOfPeople? EstimatedPeopleImpacted { get; set; }
        [Mandatory]
        public string? CommunityImpact { get; set; }
        [Mandatory]
        public bool? IsInfrastructureImpacted { get; set; }
        [MandatoryIf("IsInfrastructureImpacted", true)]
        public IEnumerable<InfrastructureImpacted>? InfrastructureImpacted { get; set; }
        public string? DisasterRiskUnderstanding { get; set; }
        public string? AdditionalBackgroundInformation { get; set; }
        public string? AddressRisksAndHazards { get; set; }
        [Mandatory]
        public string? ProjectDescription { get; set; }
        public string? AdditionalSolutionInformation { get; set; }
        [Mandatory]
        public string? RationaleForSolution { get; set; }

        //Engagement Plan - 6
        public string? FirstNationsEngagement { get; set; }
        public string? NeighbourEngagement { get; set; }
        public string? AdditionalEngagementInformation { get; set; }

        //Other Supporting Information - 7
        public string? ClimateAdaptation { get; set; }
        public string? OtherInformation { get; set; }
    }

    public class EoiApplication : DraftEoiApplication
    {
        //Declaration
        public bool? AuthorizedRepresentativeStatement { get; set; }
        public bool? FOIPPAConfirmation { get; set; }
        public bool? InformationAccuracyStatement { get; set; }
    }

    public class DraftFpApplication : DraftApplication
    {
        public string? EoiId { get; set; }
        public string? Id { get; set; }
        public SubmissionPortalStatus? Status { get; set; }
        public ContactDetails? Submitter { get; set; }

        //Proponent & Project Information - 1
        [Mandatory]
        public ContactDetails? ProjectContact { get; set; }
        [Mandatory]
        public override IEnumerable<ContactDetails> AdditionalContacts { get; set; }
        [CollectionStringLengthValid(ErrorMessage = "PartneringProponents have a limit of 40 characters per name")]
        public IEnumerable<string> PartneringProponents { get; set; }
        public bool? RegionalProject { get; set; }
        [MandatoryIf("RegionalProject", true)]
        public string? RegionalProjectComments { get; set; }
        [Mandatory]
        public string? ProjectTitle { get; set; }
        public string? MainDeliverable { get; set; }

        //Ownership & Authorization - 2
        public bool? OwnershipDeclaration { get; set; }
        [MandatoryIf("OwnershipDeclaration", false)]
        public string? OwnershipDescription { get; set; }
        public bool? HaveAuthorityToDevelop { get; set; }
        public YesNoOption? OperationAndMaintenance { get; set; }
        public string? OperationAndMaintenanceComments { get; set; }
        [Mandatory]
        public YesNoOption? FirstNationsAuthorizedByPartners { get; set; }
        [Mandatory]
        public YesNoOption? LocalGovernmentAuthorizedByPartners { get; set; }
        [Mandatory]
        public string? AuthorizationOrEndorsementComments { get; set; }

        //Project Area - 3
        [Mandatory]
        public string? LocationDescription { get; set; }
        public int? Area { get; set; }
        public AreaUnits? Units { get; set; }
        [Mandatory]
        public IEnumerable<Hazards>? RelatedHazards { get; set; }
        [MandatoryIfAny(Values = new[] { nameof(AreaUnits.Acres), nameof(AreaUnits.Hectares), nameof(AreaUnits.SqKm) }, PropertyName = nameof(Units))]
        public string? AreaDescription { get; set; }
        public string? OtherHazardsDescription { get; set; }
        [Mandatory]
        public string? CommunityImpact { get; set; }
        [Mandatory]
        public EstimatedNumberOfPeopleFP? EstimatedPeopleImpactedFP { get; set; }
        [Mandatory]
        public bool? IsInfrastructureImpacted { get; set; }
        [MandatoryIf("IsInfrastructureImpacted", true)]
        public IEnumerable<InfrastructureImpacted>? InfrastructureImpacted { get; set; }

        //Project Plan - 4
        [Mandatory]
        public DateTime? StartDate { get; set; }
        [Mandatory]
        public DateTime? EndDate { get; set; }
        [Mandatory]
        public string? ProjectDescription { get; set; }
        public IEnumerable<ProposedActivity>? ProposedActivities { get; set; }
        public IEnumerable<string>? VerificationMethods { get; set; }
        [Mandatory]
        public string? HowWasNeedIdentified { get; set; }
        public string? AddressRisksAndHazards { get; set; }
        public string? DisasterRiskUnderstanding { get; set; }
        public string? RationaleForFunding { get; set; }
        [Mandatory]
        public string? ProjectAlternateOptions { get; set; }

        //Project Engagement - 5
        public bool? EngagedWithFirstNationsOccurred { get; set; }
        [Mandatory]
        public string? EngagedWithFirstNationsComments { get; set; }
        public YesNoOption? OtherEngagement { get; set; }
        [MandatoryIf("OtherEngagement", YesNoOption.Yes)]
        public IEnumerable<string>? AffectedParties { get; set; }
        [Mandatory]
        public string? OtherEngagementComments { get; set; }
        [Mandatory]
        public string? CollaborationComments { get; set; }

        //Climate Adaptation - 6
        public bool? IncorporateFutureClimateConditions { get; set; }
        public string? ClimateAdaptation { get; set; }
        public bool? ClimateAssessment { get; set; }
        [MandatoryIf("ClimateAssessment", true)]
        public IEnumerable<string>? ClimateAssessmentTools { get; set; }
        [MandatoryIf("ClimateAssessment", true)]
        public string? ClimateAssessmentComments { get; set; }

        //Permits Regulations & Standards - 7
        public YesNoOption? StandardsAcceptable { get; set; }
        public IEnumerable<StandardInfo>? Standards { get; set; }
        public string? StandardsComments { get; set; }
        [Mandatory]
        public bool? ProfessionalGuidance { get; set; }
        [MandatoryIf("ProfessionalGuidance", true)]
        public IEnumerable<string>? Professionals { get; set; }
        public string? ProfessionalGuidanceComments { get; set; }
        [Mandatory]
        public bool? MeetsRegulatoryRequirements { get; set; }
        [MandatoryIf("MeetsRegulatoryRequirements", true)]
        public string? MeetsRegulatoryComments { get; set; }
        [Mandatory]
        public bool? MeetsEligibilityRequirements { get; set; }
        [MandatoryIf("MeetsEligibilityRequirements", true)]
        public string? MeetsEligibilityComments { get; set; }

        //Project Outcomes - 8
        [Mandatory]
        public bool? PublicBenefit { get; set; }
        [Mandatory]
        public string? PublicBenefitComments { get; set; }
        [Mandatory]
        public bool? FutureCostReduction { get; set; }
        [MandatoryIf("FutureCostReduction", true)]
        public IEnumerable<string>? CostReductions { get; set; }
        [MandatoryIf("FutureCostReduction", true)]
        public string? CostReductionComments { get; set; }
        [Mandatory]
        public bool? ProduceCoBenefits { get; set; }
        [MandatoryIf("ProduceCoBenefits", true)]
        public IEnumerable<string>? CoBenefits { get; set; }
        [MandatoryIf("ProduceCoBenefits", true)]
        public string? CoBenefitComments { get; set; }
        [Mandatory]
        public IEnumerable<string>? IncreasedResiliency { get; set; }
        [Mandatory]
        public string? IncreasedResiliencyComments { get; set; }

        //Project Risks - 9
        [Mandatory]
        public bool? ComplexityRiskMitigated { get; set; }
        [MandatoryIf("ComplexityRiskMitigated", true)]
        public IEnumerable<string>? ComplexityRisks { get; set; }
        [MandatoryIf("ComplexityRiskMitigated", true)]
        public string? ComplexityRiskComments { get; set; }
        [Mandatory]
        public bool? ReadinessRiskMitigated { get; set; }
        [MandatoryIf("ReadinessRiskMitigated", true)]
        public IEnumerable<string>? ReadinessRisks { get; set; }
        [MandatoryIf("ReadinessRiskMitigated", true)]
        public string? ReadinessRiskComments { get; set; }
        [Mandatory]
        public bool? SensitivityRiskMitigated { get; set; }
        [MandatoryIf("SensitivityRiskMitigated", true)]
        public IEnumerable<string>? SensitivityRisks { get; set; }
        [MandatoryIf("SensitivityRiskMitigated", true)]
        public string? SensitivityRiskComments { get; set; }
        [Mandatory]
        public bool? CapacityRiskMitigated { get; set; }
        [MandatoryIf("CapacityRiskMitigated", true)]
        public IEnumerable<string>? CapacityRisks { get; set; }
        [MandatoryIf("CapacityRiskMitigated", true)]
        public string? CapacityRiskComments { get; set; }
        [Mandatory]
        public bool? RiskTransferMigigated { get; set; }
        //[MandatoryIf("RiskTransferMigigated", true)]
        public IEnumerable<string>? TransferRisks { get; set; } //This is not a list in CRM. Update this
        [MandatoryIf("RiskTransferMigigated", true)]
        public string? TransferRisksComments { get; set; }

        //Budget - 10
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? TotalProjectCost { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? EligibleFundingRequest { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? RemainingAmount { get; set; }
        public IEnumerable<YearOverYearFunding>? YearOverYearFunding { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? TotalDrifFundingRequest { get; set; }
        public string? DiscrepancyComment { get; set; }
        [Mandatory]
        public bool? HaveOtherFunding { get; set; }
        [MandatoryIf("HaveOtherFunding", true)]
        public IEnumerable<FundingInformation> OtherFunding { get; set; }
        public string? IntendToSecureFunding { get; set; }
        [Mandatory]
        public string? CostEffectiveComments { get; set; }
        [Mandatory]
        public YesNoOption? PreviousResponse { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        [MandatoryIf("PreviousResponse", YesNoOption.Yes)]
        public decimal? PreviousResponseCost { get; set; }
        [MandatoryIf("PreviousResponse", YesNoOption.Yes)]
        public string? PreviousResponseComments { get; set; }
        [Mandatory]
        public bool? CostConsiderationsApplied { get; set; }
        [MandatoryIf("CostConsiderationsApplied", true)]
        public IEnumerable<string>? CostConsiderations { get; set; }
        [MandatoryIf("CostConsiderationsApplied", true)]
        public string? CostConsiderationsComments { get; set; }

        //Attachments - 11
        public IEnumerable<Attachment>? Attachments { get; set; }

        //Review & Declaration - 12
    }

    public class FpApplication : DraftFpApplication
    {

    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class ProposedActivity
    {
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? RelatedMilestone { get; set; }
    }

    public class YearOverYearFunding
    {
        public string? Year { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? Amount { get; set; }
    }

    public class FundingInformation
    {
        public string? Name { get; set; }
        public FundingType? Type { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? Amount { get; set; }
        public string? OtherDescription { get; set; }
    }

    public class InfrastructureImpacted
    {
        public string? Infrastructure { get; set; }
        public string? Impact { get; set; }
    }

    public class StandardInfo
    {
        public bool? IsCategorySelected { get; set; }
        public string? Category { get; set; }
        public IEnumerable<string>? Standards { get; set; }
    }

    public class ContactDetails
    {
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public string? FirstName { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public string? LastName { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public string? Title { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public string? Department { get; set; }
        //[RegularExpression("^\\d\\d\\d-\\d\\d\\d-\\d\\d\\d\\d$", ErrorMessage = "Phone number must be of the format '000-000-0000'")]
        public string? Phone { get; set; }
        [StringLength(ApplicationValidators.CONTACT_MAX_LENGTH)]
        public string? Email { get; set; }
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
    public enum EstimatedNumberOfPeopleFP
    {
        [Description("0 - 500")]
        ZeroToFiveHundred,

        [Description("501 - 1,000")]
        FiveHundredToOneK,

        [Description("1,001 - 5,000")]
        OneKToFiveK,

        [Description("5,001 - 10,000")]
        FiveKToTenK,

        [Description("10,001 - 50,000")]
        TenKToFiftyK,

        [Description("50,001 - 100k")]
        FiftyKToHundredK,

        [Description("100,001 +")]
        HundredKPlus,

        [Description("Unsure")]
        Unsure
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum YesNoOption
    {
        No,
        Yes,
        NotApplicable
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AreaUnits
    {
        [Description("Hectares")]
        Hectares,

        [Description("Acres")]
        Acres,

        [Description("Square Kms")]
        SqKm
    }

#pragma warning disable CS8765 // nullability
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public class Mandatory : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;

            if ((model.GetType() != typeof(DraftEoiApplication) && model.GetType() != typeof(DraftFpApplication)) && value == null)
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                return new ValidationResult($"{propertyInfo.Name} is required");
            }
            return ValidationResult.Success;
        }
    }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
    public class MandatoryIf : ValidationAttribute
    {
        RequiredAttribute _innerAttribute = new RequiredAttribute();
        public string _dependentProperty { get; set; }
        public object _targetValue { get; set; }

        public MandatoryIf(string dependentProperty, object targetValue)
        {
            this._dependentProperty = dependentProperty;
            this._targetValue = targetValue;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;

            if ((model.GetType() == typeof(FpApplication) || model.GetType() == typeof(EoiApplication)) && value == null)
            {

                var field = validationContext.ObjectType.GetProperty(_dependentProperty);
                if (field != null)
                {
                    var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                    if ((dependentValue == null && _targetValue == null) || (dependentValue != null && dependentValue.Equals(_targetValue)))
                    {
                        if (!_innerAttribute.IsValid(value))
                        {
                            string name = validationContext.DisplayName;
                            string specificErrorMessage = ErrorMessage;
                            if (specificErrorMessage != null && specificErrorMessage.Length < 1)
                                specificErrorMessage = $"{name} is required.";

                            return new ValidationResult(specificErrorMessage, new[] { validationContext.MemberName });
                        }
                    }
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(_dependentProperty));
                }
            }
            return ValidationResult.Success;
        }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MandatoryIfAny : ValidationAttribute
    {
        public string[] Values { get; set; }
        public string PropertyName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;
            if (model == null || Values == null)
            {
                return ValidationResult.Success;
            }

            if ((model.GetType() == typeof(FpApplication) || model.GetType() == typeof(EoiApplication)) && value == null)
            {
                var currentValue = model.GetType().GetProperty(PropertyName)?.GetValue(model, null)?.ToString();
                if (Values.Contains(currentValue) && value == null)
                {
                    var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                    return new ValidationResult($"{propertyInfo.Name} is required for the current {PropertyName} value {currentValue}");
                }
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class CollectionStringLengthValid : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (!(value is IList)) return false;
            foreach (string item in (IList)value)
            {
                if (item?.Length > ApplicationValidators.ACCOUNT_MAX_LENGTH) return false;
            }
            return true;
        }
    }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning restore IDE0300 // Simplify collection initialization
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8765
}
