

using System.ComponentModel;
using EMCR.DRR.Controllers;

namespace EMCR.DRR.Managers.Intake
{
    public interface IIntakeManager
    {
        Task<string> Handle(IntakeCommand cmd);
    }

    public abstract class IntakeCommand
    { }

    public class DrifEoiApplicationCommand : IntakeCommand
    {
        public DrifEoiApplication application { get; set; } = null!;
    }

    public class Application
    {
        //Proponent Information
        public ProponentType ProponentType { get; set; }
        public required string ProponentName { get; set; }
        public required ContactDetails Submitter { get; set; }
        public required ContactDetails ProjectContact { get; set; }
        public ContactDetails? AdditionalContact1 { get; set; }
        public ContactDetails? AdditionalContact2 { get; set; }
        public required IEnumerable<PartneringProponent> PartneringProponents { get; set; }

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
        public required decimal EstimatedTotal { get; set; }
        public required decimal FundingRequest { get; set; }
        public required IEnumerable<FundingInformation> OtherFunding { get; set; }
        public required decimal RemainingAmount { get; set; }
        public string? IntendToSecureFunding { get; set; }

        //Location Information
        public required bool OwnershipDeclaration { get; set; }
        public required string OwnershipDescription { get; set; }
        public required string LocationDescription { get; set; }

        //Project Detail
        public required string RationaleForFunding { get; set; }
        public required int EstimatedPeopleImpacted { get; set; }
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
        public required bool FOIPPAConfirmation { get; set; }
        public required bool FinancialAwarenessConfirmation { get; set; }
    }

    public class FundingInformation
    {
        public required string Name { get; set; }
        public required FundingType Type { get; set; }
        public required decimal Amount { get; set; }
        public string? OtherDescription { get; set; }

    }

    public class LocationInformation
    {
        public required string Description { get; set; }
        public string? Area { get; set; }
        public string? Ownership { get; set; }
    }

    public class ContactDetails
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Title { get; set; }
        public required string Department { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
    }

    public class PartneringProponent
    {
        public required string Name { get; set; }
    }

    public enum ProponentType
    {
        FirstNation,
        LocalGovernment,
        RegionalDistrict
    }

    public enum FundingStream
    {
        Stream1,
        Stream2
    }

    public enum ProjectType
    {
        New,
        Existing
    }


    public enum FundingType
    {
        Fed,
        FedProv,
        Prov,
        SelfFunding,
        Other,
    }

    public enum Hazards
    {
        Drought,
        ExtremeTemperature,
        Flood,
        Geohazards,
        SeaLevelRise,
        Seismic,
        Tsunami,
        Other,
    }
}
