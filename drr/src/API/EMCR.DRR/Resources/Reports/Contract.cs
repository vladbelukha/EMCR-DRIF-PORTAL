using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;

namespace EMCR.DRR.API.Resources.Reports
{
    public interface IReportRepository
    {
        Task<ReportQueryResult> Query(ReportQuery query);
        Task<ClaimQueryResult> Query(ClaimQuery query);
        Task<ProgressReportQueryResult> Query(ProgressReportQuery query);
        Task<ForecastQueryResult> Query(ForecastQuery query);
        //Task<bool> CanAccessProject(string id, string businessId);
    }

    public abstract class ReportQuery
    { }

    public class ReportQueryResult
    {
        public IEnumerable<InterimReportDetails> Items { get; set; } = Array.Empty<InterimReportDetails>();
        public int Length { get; set; }
    }

    public class ReportsQuery : ReportQuery
    {
        public string? Id { get; set; }
        public string? BusinessId { get; set; }
    }

    public abstract class ClaimQuery
    { }

    public class ClaimQueryResult
    {
        public IEnumerable<ClaimDetails> Items { get; set; } = Array.Empty<ClaimDetails>();
        public int Length { get; set; }
    }

    public class ClaimsQuery : ClaimQuery
    {
        public string? Id { get; set; }
        public string? BusinessId { get; set; }
    }

    public abstract class ProgressReportQuery
    { }

    public class ProgressReportQueryResult
    {
        public IEnumerable<ProgressReportDetails> Items { get; set; } = Array.Empty<ProgressReportDetails>();
        public int Length { get; set; }
    }

    public class ProgressReportsQuery : ProgressReportQuery
    {
        public string? Id { get; set; }
        public string? BusinessId { get; set; }
    }

    public abstract class ForecastQuery
    { }

    public class ForecastQueryResult
    {
        public IEnumerable<ForecastDetails> Items { get; set; } = Array.Empty<ForecastDetails>();
        public int Length { get; set; }
    }

    public class ForecastsQuery : ForecastQuery
    {
        public string? Id { get; set; }
        public string? BusinessId { get; set; }
    }
}
