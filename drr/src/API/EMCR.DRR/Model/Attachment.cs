using System.Text.Json.Serialization;

namespace EMCR.DRR.API.Model
{
    public class Attachment
    {
        public string? Id { get; set; }
        public required string ApplicationId { get; set; }
        public required string Name { get; set; }
        public byte[]? Body { get; set; }
        public string? Comments { get; set; }
        public DocumentType DocumentType { get; set; }
        public bool? HaveResolution { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DocumentType
    {
        DetailedProjectWorkplan,
        ProjectSchedule,
        DetailedCostEstimate,
        SitePlan,
        PreliminaryDesign,
        FirstNationsResolution,
        LocalGovernmentResolution,
        OtherSupportingDocument,
    }
}
