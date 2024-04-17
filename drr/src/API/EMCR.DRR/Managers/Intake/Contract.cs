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
        public ApplicantType ApplicantType { get; set; }
        public required string ApplicantName { get; set; }
        public required ContactDetails Submitter { get; set; }
        public required IEnumerable<ContactDetails> ProjectContacts { get; set; }
        public required string ProjectTitle { get; set; }
        public ProjectType ProjectType { get; set; }
        public required IEnumerable<Hazards> RelatedHazards { get; set; }
        public string? OtherHazardsDescription { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required decimal FundingRequest { get; set; }
        public required IEnumerable<FundingInformation> OtherFunding { get; set; }
        public required decimal UnfundedAmount { get; set; }
        public string? ReasonsToSecureFunding { get; set; }
        public required decimal TotalFunding { get; set; }
        public required bool OwnershipDeclaration { get; set; }
        public required LocationInformation LocationInformation { get; set; }
        public required string BackgroundDescription { get; set; }
        public required string RationaleForFunding { get; set; }
        public required string ProposedSolution { get; set; }
        public required string RationaleForSolution { get; set; }
        public required string EngagementProposal { get; set; }
        public required string ClimateAdaptation { get; set; }
        public required string OtherInformation { get; set; }
        public required bool IdentityConfirmation { get; set; }
        public required bool FOIPPAConfirmation { get; set; }
        public required bool CFOConfirmation { get; set; }
    }

    public class FundingInformation
    {
        public required string Name { get; set; }
        public required FundingType Type { get; set; }
        public required decimal Amount { get; set; }

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

    public enum ApplicantType
    {
        FirstNation,
        LocalGovernment,
        RegionalDistrict
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
        Erosion,
        ExtremeTemperature,
        Flood,
        Geohazards,
        SeaLevelRise,
        Seismic,
        Storm,
        Tsunami,
        Other,
    }
}
