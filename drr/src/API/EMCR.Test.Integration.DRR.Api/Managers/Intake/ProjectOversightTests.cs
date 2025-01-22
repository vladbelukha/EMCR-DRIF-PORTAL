using System.Text;
using AutoMapper;
using EMCR.DRR.API.Model;
using EMCR.DRR.API.Services.S3;
using EMCR.DRR.Controllers;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using static EMCR.DRR.Controllers.ProjectController;

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
            projects.ShouldAllBe(s => s.ProgramType == ProgramType.DRIF);
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
            var queryRes = await manager.Handle(new DrrProgressReportsQuery { Id = "DRIF-PR-1051", BusinessId = GetTestUserInfo().BusinessId });
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
    }
}
