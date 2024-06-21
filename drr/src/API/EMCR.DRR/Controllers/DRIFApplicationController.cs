using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json.Serialization;
using AutoMapper;
using EMCR.DRR.API.Model;
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
        private readonly IMapper mapper;

        public DRIFApplicationController(ILogger<DRIFApplicationController> logger, IIntakeManager intakeManager, IMapper mapper)
        {
            this.logger = logger;
            this.intakeManager = intakeManager;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Submission>>> Get()
        {
            await Task.CompletedTask;
            var result = new[]
            {
                new Submission
                {
                    Id = Guid.NewGuid(),
                    ProjectTitle = "Project 1",
                    Status = SubmissionPortalStatus.Draft,
                    FundingRequest = "1000",
                    ModifiedDate = DateTime.Now.AddDays(-1),
                    SubmittedDate = DateTime.Now.AddDays(-1),
                    PartneringProponents = new[] { "Partner 1", "Partner 2" }
                },
                new Submission
                {
                    Id = Guid.Parse("14dabac4-e399-456c-8d48-555a9007e31e"),
                    ProjectTitle = "Project 2",
                    Status = SubmissionPortalStatus.Submitted,
                    FundingRequest = "2000",
                    ModifiedDate = DateTime.Now.AddDays(-1),
                    SubmittedDate = DateTime.Now.AddDays(-2),
                    PartneringProponents = new[] { "Partner 3", "Partner 4", "Partner 5" }
                },
                new Submission
                {
                    Id = Guid.NewGuid(),
                    ProjectTitle = "Project 3",
                    Status = SubmissionPortalStatus.Draft,
                    FundingRequest = "3000",
                    ModifiedDate = DateTime.Now.AddDays(-5),
                    SubmittedDate = DateTime.Now.AddDays(-10),
                    PartneringProponents = new[] { "Partner 5", "Partner 6" }
                },
                new Submission
                {
                    Id = Guid.NewGuid(),
                    ProjectTitle = "Project 4",
                    Status = SubmissionPortalStatus.Rejected,
                    FundingRequest = "4000",
                    ModifiedDate = DateTime.Now.AddDays(-2),
                    SubmittedDate = DateTime.Now.AddDays(-3),
                    PartneringProponents = new[] { "Partner 7" }
                },
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DrifEoiApplication>> Get(Guid id)
        {
            // TODO: remove after
            // compare if GUID is 14dabac4-e399-456c-8d48-555a9007e31e
            var status = id.ToString() == "14dabac4-e399-456c-8d48-555a9007e31e" ? SubmissionPortalStatus.Submitted : SubmissionPortalStatus.Draft;

            await Task.CompletedTask;
            return Ok(CreateNewTestEOIApplication(id, status));
        }

        [HttpGet("Declarations")]
        public async Task<ActionResult<DeclarationResult>> GetDeclarations()
        {
            var res = await intakeManager.Handle(new DeclarationQuery());

            return Ok(new DeclarationResult { Items = mapper.Map<IEnumerable<DeclarationInfo>>(res.Items) });
        }

        [HttpPost("EOI")]
        public async Task<ActionResult<ApplicationResult>> CreateEOIApplication(DrifEoiApplication application)
        {
            var id = await intakeManager.Handle(new DrifEoiApplicationCommand { application = application });
            return Ok(new ApplicationResult { Id = id });
        }

        [HttpPost("EOI/draft")]
        public async Task<ActionResult<ApplicationResult>> CreateDraftEOIApplication(DrifDrafEoiApplication application)
        {
            var id = "DRIF-EOI-####";
            await Task.CompletedTask;
            return Ok(new ApplicationResult { Id = id });
        }

        //THESE ARE FOR THE MOCK API CALLS ONLY - TO BE REMOVED!
        private string[] firstNames = new[] { "John", "Ashlynn", "Hector", "Ariah", "Ashlynn", "Talon", "Mckenna", "Kamden", "Chaya", "Vincenzo", "Freya", "Azrael", "Leslie", "Hector", "Daleyza", "Kayson", "Scarlet", "Joshua", "Anika", "Christopher", "Opal", "Martin", "Catherine", "Aiden", "Mckinley", "Adam", "Charlotte", "Barrett", "Raelynn", "Keaton", "Katie", "Joe", "Jovie", "David", "Viviana", "Mason", "Jayda", "Aldo", "Halo", "Conner", "Kadence", "Corey", "Selene", "Elijah", "Blair", "Callen", "Charli", "Jack", "Denver", "Enzo", "Brinley", "Major", "Allie", "Waylon", "Alaiya", "Kingston", "Freya", "Jax", "Sophia", "Bruce", "Jayda", "Emanuel", "Harmony", "Kayden", "Hana", "Zakai", "Isabella", "Sage", "Ezra", "Beckett", "Stormi", "Parker", "Eva", "Titan", "Persephone", "Joel", "Ariah", "Kayden", "Eve", "Adonis", "Dylan", "Finn", "Dalary", "Uriah", "Braelynn", "Hugo", "Samira", "Blaise", "Mercy", "Koda", "Brylee", "Mordechai", "Alina", "Kayden", "Amber", "Niklaus", "Luciana", "Sincere", "Monica", "Mitchell", "Lena", "Soren", "Virginia", "Jagger", };
        private string[] lastNames = new[] { "Gaines", "Howell", "Rowe", "Boyer", "Preston", "Salazar", "Xiong", "Zuniga", "Hail", "Cabrera", "Day", "Walton", "Turner", "Kemp", "Campbell", "Dickerson", "Banks", "Williamson", "White", "Mercado", "Myers", "Brown", "Soto", "Russell", "Savage", "Ingram", "Stephenson", "Finley", "Ramirez", "Chandler", "Hernandez", "Wall", "Marin", "McCarty", "McGee", "Archer", "French", "Sloan", "Brown", "Ayala", "Norton", "O’Connor", "Lee", "Herring", "Wagner", "Hale", "McDaniel", "Serrano", "Perry", "Orr", "Alexander", "Salazar", "Burke", "Jones", "Colon", "Wall", "Dennis", "Robertson", "Foster", "McCullough", "Correa", "Miller", "Garner", "Beard", "Knight", "Stuart", "Patel", "Castillo", "Rubio", "Howe", "Hart", "Aguirre", "Foster", "Hodges", "Berry", "Person", "Stone", "Parra", "Bruce", "Newton", "Swanson", "Lynn", "McCarty", "Sellers", "Greer", "Pruitt", "Richmond", "Hicks", "Foster", "Salas", "Moses", "Ochoa", "Zuniga", "Marks", "McClain", "Sims", "Suarez", "Hoover", "Keith", "Roth", };
        private string GenerateName => $"{firstNames[Random.Shared.Next(0, firstNames.Length)]} {lastNames[Random.Shared.Next(0, lastNames.Length)]}";

        private DrifEoiApplication CreateNewTestEOIApplication(Guid id, SubmissionPortalStatus status)
        {

            var proponentName = $"{firstNames[Random.Shared.Next(0, firstNames.Length)]} {lastNames[Random.Shared.Next(0, lastNames.Length)]}";
            return new DrifEoiApplication
            {
                Status = status,

                //Proponent Information
                ProponentType = ProponentType.LocalGovernment,
                ProponentName = GenerateName,
                Submitter = CreateNewTestContact(),
                ProjectContact = CreateNewTestContact(),
                AdditionalContacts = new[]
                {
                    CreateNewTestContact(),
                    CreateNewTestContact(),
                },
                PartneringProponents = new[]
                {
                    GenerateName,
                    GenerateName
                },

                //Project Information
                FundingStream = FundingStream.Stream1,
                ProjectTitle = "Project Title",
                ProjectType = ProjectType.New,
                ScopeStatement = "scope",
                RelatedHazards = new[]
                {
                    Hazards.Flood,
                    Hazards.Tsunami,
                    Hazards.Other
                },
                OtherHazardsDescription = "Other Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),

                //Funding Information
                EstimatedTotal = 1000,
                FundingRequest = 100,
                OtherFunding = new[]
                {
                    new FundingInformation
                    {
                        Name = $"Self",
                        Amount = 100,
                        Type = FundingType.SelfFunding,
                    },
                    new FundingInformation
                    {
                        Name = $"Prov",
                        Amount = 200,
                        Type = FundingType.Prov,
                    },
                    new FundingInformation
                    {
                        Name = $"Other",
                        Amount = 300,
                        Type = FundingType.OtherGrants,
                        OtherDescription = "other funding reason"
                    },
                },
                RemainingAmount = 600,
                IntendToSecureFunding = "Funding Reasons",

                //Location Information
                OwnershipDeclaration = true,
                OwnershipDescription = "owned",
                LocationDescription = "location description",

                //Project Detail
                RationaleForFunding = "rationale for funding",
                EstimatedPeopleImpacted = EstimatedNumberOfPeople.OneToTenK,
                CommunityImpact = "community impact",
                InfrastructureImpacted = new[] { $"infrastructure" },
                DisasterRiskUnderstanding = "helps many people",
                AdditionalBackgroundInformation = "additional background info",
                AddressRisksAndHazards = "fix risks",
                DRIFProgramGoalAlignment = "aligns with goals",
                AdditionalSolutionInformation = "additional solution info",
                RationaleForSolution = "rational for solution",

                //Engagement Plan
                FirstNationsEngagement = "Engagement Proposal",
                NeighbourEngagement = "engage with neighbours",
                AdditionalEngagementInformation = "additional engagement info",

                //Other Supporting Information
                ClimateAdaptation = "Climate Adaptation",
                OtherInformation = "Other Info",

                //Declaration
                InformationAccuracyStatement = true,
                //FOIPPAConfirmation = true,
                AuthorizedRepresentativeStatement = true
            };
        }

        private ContactDetails CreateNewTestContact()
        {
            var firstName = firstNames[Random.Shared.Next(0, firstNames.Length)];
            var lastName = lastNames[Random.Shared.Next(0, lastNames.Length)];
            return new ContactDetails
            {
                FirstName = firstName,
                LastName = lastName,
                Email = $"{firstName}.{lastName}@test.com",
                Phone = "604-123-4567",
                Department = "Position",
                Title = "Title"
            };
        }
    }

    public class DeclarationResult
    {
        public IEnumerable<DeclarationInfo> Items { get; set; } = Array.Empty<DeclarationInfo>();
    }

    public class DeclarationInfo
    {
        public required DeclarationType Type { get; set; }
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

    public class DrifDrafEoiApplication
    {
        //Proponent Information
        public ProponentType? ProponentType { get; set; }
        public string? ProponentName { get; set; }
        public DraftContactDetails? Submitter { get; set; }
        public DraftContactDetails? ProjectContact { get; set; }
        public IEnumerable<DraftContactDetails>? AdditionalContacts { get; set; }
        [CollectionStringLengthValid(ErrorMessage = "PartneringProponents have a limit of 40 characters per name")]
        public IEnumerable<string>? PartneringProponents { get; set; }

        //Project Information
        public FundingStream? FundingStream { get; set; }
        public string? ProjectTitle { get; set; }
        public ProjectType? ProjectType { get; set; }
        public string? ScopeStatement { get; set; }
        public IEnumerable<Hazards>? RelatedHazards { get; set; }
        public string? OtherHazardsDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //Funding Information
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? EstimatedTotal { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? FundingRequest { get; set; }
        public IEnumerable<DraftFundingInformation>? OtherFunding { get; set; }
        public decimal? RemainingAmount { get; set; }
        public string? IntendToSecureFunding { get; set; }

        //Location Information
        public bool? OwnershipDeclaration { get; set; }
        public string? OwnershipDescription { get; set; }
        public string? LocationDescription { get; set; }

        //Project Detail
        public string? RationaleForFunding { get; set; }
        public EstimatedNumberOfPeople? EstimatedPeopleImpacted { get; set; }
        public string? CommunityImpact { get; set; }
        public IEnumerable<string>? InfrastructureImpacted { get; set; }
        public string? DisasterRiskUnderstanding { get; set; }
        public string? AdditionalBackgroundInformation { get; set; }
        public string? AddressRisksAndHazards { get; set; }
        public string? DRIFProgramGoalAlignment { get; set; }
        public string? AdditionalSolutionInformation { get; set; }
        public string? RationaleForSolution { get; set; }

        //Engagement Plan
        public string? FirstNationsEngagement { get; set; }
        public string? NeighbourEngagement { get; set; }
        public string? AdditionalEngagementInformation { get; set; }

        //Other Supporting Information
        public string? ClimateAdaptation { get; set; }
        public string? OtherInformation { get; set; }


        //Declaration
        public bool? AuthorizedRepresentativeStatement { get; set; }
        public bool? FOIPPAConfirmation { get; set; }
        public bool? InformationAccuracyStatement { get; set; }
    }

    public class DraftContactDetails
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

    public class DraftFundingInformation
    {
        public string? Name { get; set; }
        public FundingType? Type { get; set; }
        [Range(0, ApplicationValidators.FUNDING_MAX_VAL)]
        public decimal? Amount { get; set; }
        public string? OtherDescription { get; set; }

    }

    public class DrifEoiApplication
    {
        public required SubmissionPortalStatus Status { get; set; }

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
        public required bool AuthorizedRepresentativeStatement { get; set; }
        public bool? FOIPPAConfirmation { get; set; }
        public required bool InformationAccuracyStatement { get; set; }
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
#pragma warning restore CS8765
}
