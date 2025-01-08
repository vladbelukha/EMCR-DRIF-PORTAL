using EMCR.DRR.API.Resources.Projects;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EMCR.Tests.Integration.DRR.Resources
{
    public class ProjectTests
    {

        private string TestPrefix = "autotest-dev";
        private string TestBusinessId = "autotest-dev-business-bceid";
        //private string TestUserId = "autotest-dev-user-bceid";
        private readonly IProjectRepository projectRepository;

        public ProjectTests()
        {
            var host = Application.Host;
            projectRepository = host.Services.GetRequiredService<IProjectRepository>();
        }

        [Test]
        public async Task CanQueryProjects()
        {
            var projects = (await projectRepository.Query(new ProjectsQuery { BusinessId = TestBusinessId })).Items;
            projects.ShouldNotBeEmpty();
        }
    }
}
