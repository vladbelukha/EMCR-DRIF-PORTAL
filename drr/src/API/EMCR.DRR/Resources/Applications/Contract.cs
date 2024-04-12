using EMCR.DRR.Controllers;

namespace EMCR.DRR.Resources.Applications
{
    public interface IApplicationRepository
    {
        Task<ManageApplicationCommandResult> Manage(ManageApplicationCommand cmd);
    }

    public abstract class ManageApplicationCommand
    { }

    public class ManageApplicationCommandResult
    {
        public required string Id { get; set; }
    }

    public class SubmitEOIApplication : ManageApplicationCommand
    {
        public required EOIApplication EOIApplication { get; set; }
    }

    public enum ApplicantTypeOptionSet
    {
        FirstNation = 172580000,
        LocalGovernment = 172580001,
        RegionalDistrict = 172580002
    }

    public enum DRRTwoOptions
    {
        Yes = 172580000,
        No = 172580001
    }
}
