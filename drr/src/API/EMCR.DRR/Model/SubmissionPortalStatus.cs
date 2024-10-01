using System.Text.Json.Serialization;

namespace EMCR.DRR.API.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SubmissionPortalStatus
    {
        Draft,
        UnderReview,
        Ineligible,
        EligiblePending,
        EligibleInvited,
        Withdrawn,
        Closed,
        FullProposalSubmitted,
        Approved,
        ApprovedInPrinciple,
        Deleted,
    }
}
