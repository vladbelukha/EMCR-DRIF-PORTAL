using System.Text.Json.Serialization;

namespace EMCR.DRR.API.Model
{
    public class FileData
    {
        public required string ApplicationId { get; set; }
        public required string Name { get; set; }
        public required string ContentType { get; set; }
        public required byte[] Content { get; set; }
        public DocumentType DocumentType { get; set; }
    }

    public class Attachment
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public DocumentType DocumentType { get; set; }
        public string? Comments { get; set; }
    }

    public class DeleteAttachment
    {
        public required string Id { get; set; }
        public required string ApplicationId { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DocumentType
    {
        DetailedProjectWorkplan,
        ProjectSchedule,
        DetailedCostEstimate,
        SitePlan,
        PreliminaryDesign,
        Resolution,
        OtherSupportingDocument,
    }
}
