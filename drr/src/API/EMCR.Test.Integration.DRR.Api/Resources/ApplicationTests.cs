using EMBC.Tests.Integration.DFA.Api;
using EMCR.DRR.Controllers;
using EMCR.DRR.Resources.Applications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EMCR.Tests.Integration.DRR.Api.Resources
{
    public class ApplicationTests
    {
        private string TestPrefix = "autotest-dev";

        [Test]
        public async Task CanCreateEOIApplication()
        {
            var host = Application.Host;
            var applicationRepository = host.Services.GetRequiredService<IApplicationRepository>();

            var originalApplication = CreateTestEOIApplication();
            var id = (await applicationRepository.Manage(new SubmitEOIApplication { EOIApplication = originalApplication })).Id;
            id.ShouldNotBeEmpty();
        }

        private EOIApplication CreateTestEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            return new EOIApplication
            {
                ApplicantType = ApplicantType.LocalGovernment,
                ApplicantName = $"{uniqueSignature}_primaryApplicant",
                Submitter = CreateTestContact(uniqueSignature),
                ProjectContacts = new[]
                {
                    CreateTestContact(uniqueSignature)
                },
                ProjectTitle = $"{uniqueSignature}_projectTitle",
                ProjectType = ProjectType.New,
                RelatedHazards = new[]
                {
                    "test",
                },
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                FundingRequest = 100,
                OtherFunding = new[]
                {
                    "test",
                },
                UnfundedAmount = 100,
                TotalFunding = 200,
                OwnershipDeclaration = true,
                LocationDescription = "location description",
                //Coordinates
                //Area
                //Units
                //Ownership
                BackgroundDescription = "background description",
                RationaleForFunding = "rationale for funding",
                ProposedSolution = "solution",
                RationaleForSolution = "rational for solution",
                EngagementProposal = "Engagement Proposal",
                ClimateAdaptation = "Climate Adaptation",
                OtherInformation = "Other Info",
                IdentityConfirmation = true,
                FOIPPAConfirmation = true,
                CFOConfirmation = true
            };
        }

        private ContactDetails CreateTestContact(string uniqueSignature)
        {
            return new ContactDetails
            {
                FirstName = $"{uniqueSignature}_first",
                LastName = $"{uniqueSignature}_last",
                Email = "test@test.com",
                Phone = "604-123-4567",
                Position = "Position",
                Title = "Title"
            };
        }
    }


}
