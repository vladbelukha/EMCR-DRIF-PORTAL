using AutoMapper;
using EMCR.DRR.API.Model;
using EMCR.DRR.Controllers;
using EMCR.DRR.Managers.Intake;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using EMCR.Utilities.Extensions;

namespace EMCR.Tests.Integration.DRR.Managers.Intake
{
    public class ProjectOversightTests
    {
        private string TestPrefix = "autotest-dev";
        private string TestBusinessId = "autotest-dev-business-bceid";
        private string TestBusinessName = "autotest-dev-business-name";
        private string TestUserId = "autotest-dev-user-bceid";

        private string CRAFTD1BusinessId = "9F4430C64A2546C08B1F129F4071C1B4";
        private string CRAFTD1BusinessName = "EMCR CRAFT BCeID DEV";
        private string CRAFTD1UserId = "FAAA14A088F94B78B121C8A025F7304D";

        private UserInfo GetTestUserInfo()
        {
            return new UserInfo { BusinessId = TestBusinessId, BusinessName = TestBusinessName, UserId = TestUserId };
        }

        private UserInfo GetCRAFTUserInfo()
        {
            return new UserInfo { BusinessId = CRAFTD1BusinessId, BusinessName = CRAFTD1BusinessName, UserId = CRAFTD1UserId };
        }
        private readonly IIntakeManager manager;
        private readonly IMapper mapper;

        public ProjectOversightTests()
        {
            var host = Application.Host;
            manager = host.Services.GetRequiredService<IIntakeManager>();
            mapper = host.Services.GetRequiredService<IMapper>();
        }

        [Test]
        public async Task QueryProjects()
        {
            var queryRes = await manager.Handle(new DrrProjectsQuery { BusinessId = GetTestUserInfo().BusinessId });
            var projects = mapper.Map<IEnumerable<DraftDrrProject>>(queryRes.Items);
            projects.Count().ShouldBeGreaterThan(1);
            //projects.ShouldAllBe(s => s.ProgramType == ProgramType.DRIF);
        }

        [Test]
        public async Task QueryProjects_CanFilterById()
        {
            var queryOptions = new QueryOptions { Filter = "programType=DRIF,applicationType=FP,status=*UnderReview\\|EligiblePending" };
            var queryRes = await manager.Handle(new DrrProjectsQuery { Id = "DRIF-PRJ-1012", BusinessId = GetCRAFTUserInfo().BusinessId, QueryOptions = queryOptions });
            var projects = mapper.Map<IEnumerable<DraftDrrProject>>(queryRes.Items);
            projects.Count().ShouldBe(1);
            //projects.ShouldAllBe(s => s.ProgramType == ProgramType.DRIF);
        }

        [Test]
        public async Task QueryReports_CanFilterById()
        {
            var queryRes = await manager.Handle(new DrrReportsQuery { Id = "DRIF-REP-1034", BusinessId = GetTestUserInfo().BusinessId });
            var reports = mapper.Map<IEnumerable<EMCR.DRR.Controllers.InterimReport>>(queryRes.Items);
            reports.Count().ShouldBe(1);
        }

        [Test]
        public async Task QueryClaims_CanFilterById()
        {
            var queryRes = await manager.Handle(new DrrClaimsQuery { Id = "DRIF-CLAIM-1000", BusinessId = GetTestUserInfo().BusinessId });
            var claims = mapper.Map<IEnumerable<EMCR.DRR.Controllers.ProjectClaim>>(queryRes.Items);
            claims.Count().ShouldBe(1);
        }

        [Test]
        public async Task QueryProgressReports_CanFilterById()
        {
            var queryRes = await manager.Handle(new DrrProgressReportsQuery { Id = "DRIF-PR-1058", BusinessId = GetTestUserInfo().BusinessId });
            var prs = mapper.Map<IEnumerable<EMCR.DRR.Controllers.ProgressReport>>(queryRes.Items);
            prs.Count().ShouldBe(1);
        }

        [Test]
        public async Task QueryForecasts_CanFilterById()
        {
            var queryRes = await manager.Handle(new DrrForecastsQuery { Id = "DRIF-FORECAST-1001", BusinessId = GetTestUserInfo().BusinessId });
            var forecasts = mapper.Map<IEnumerable<EMCR.DRR.Controllers.Forecast>>(queryRes.Items);
            forecasts.Count().ShouldBe(1);
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
        [Test]
        public async Task CanUpdateProgressReport()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);

            var progressReport = mapper.Map<EMCR.DRR.Controllers.ProgressReport>((await manager.Handle(new DrrProgressReportsQuery { Id = "DRIF-PR-1058", BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault());
            progressReport.Workplan.MediaAnnouncementComment = $"{uniqueSignature} - media comment";

            progressReport.Workplan.ProjectProgress = EMCR.DRR.Controllers.ProjectProgress.OnSchedule;
            progressReport.Workplan.ProjectCompletionPercentage = (decimal?)12.5;
            progressReport.Workplan.ConstructionCompletionPercentage = (decimal?)35.7;
            progressReport.Workplan.SignageRequired = true;
            progressReport.Workplan.MediaAnnouncement = true;
            progressReport.Workplan.MediaAnnouncementDate = DateTime.UtcNow.AddDays(3);
            progressReport.Workplan.MediaAnnouncementComment = "media announcement description";
            progressReport.Workplan.OutstandingIssues = true;
            progressReport.Workplan.OutstandingIssuesComments = "issues description";
            progressReport.Workplan.FundingSourcesChanged = true;
            progressReport.Workplan.FundingSourcesChangedComment = "funding change description";


            if (progressReport.Workplan.WorkplanActivities.Length > 0) progressReport.Workplan.WorkplanActivities = progressReport.Workplan.WorkplanActivities.Take(progressReport.Workplan.WorkplanActivities.Count() - 1).ToArray();
            progressReport.Workplan.WorkplanActivities = progressReport.Workplan.WorkplanActivities.Append(new WorkplanActivity
            {
                Activity = EMCR.DRR.Controllers.ActivityType.Mapping,
                ActualCompletionDate = DateTime.UtcNow.AddDays(11),
                ActualStartDate = DateTime.UtcNow.AddDays(4),
                Comment = $"{uniqueSignature} - mapping comment",
                Id = Guid.NewGuid().ToString(),
                PlannedCompletionDate = DateTime.UtcNow.AddDays(10),
                PlannedStartDate = DateTime.UtcNow.AddDays(3),
                Status = EMCR.DRR.Controllers.WorkplanStatus.NotStarted,
            }).ToArray();
            if (progressReport.Workplan.FundingSignage.Length > 0) progressReport.Workplan.FundingSignage = progressReport.Workplan.FundingSignage.Take(progressReport.Workplan.FundingSignage.Count() - 1).ToArray();
            progressReport.Workplan.FundingSignage = progressReport.Workplan.FundingSignage.Append(new EMCR.DRR.Controllers.FundingSignage
            {
                Id = Guid.NewGuid().ToString(),
                SignageType = EMCR.DRR.Controllers.SignageType.Temporary,
                DateInstalled = DateTime.UtcNow.AddDays(3),
                DateRemoved = DateTime.UtcNow.AddDays(7),
                BeenApproved = false,
            }).ToArray();

            //Console.WriteLine(progressReport.Id);
            await manager.Handle(new SaveProgressReportCommand { ProgressReport = progressReport, UserInfo = GetTestUserInfo() });


            var updatedProgressReport = mapper.Map<EMCR.DRR.Controllers.ProgressReport>((await manager.Handle(new DrrProgressReportsQuery { Id = progressReport.Id, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault());
            updatedProgressReport.Workplan.MediaAnnouncementComment.ShouldBe(progressReport.Workplan.MediaAnnouncementComment);
        }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}
