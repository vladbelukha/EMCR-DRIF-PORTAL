using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EMCR.Tests.Integration.DRR.Resources
{
    public class ApplicationTests
    {
        private string TestPrefix = "autotest-dev";

        [Test]
        public async Task CanCreateEOIApplication()
        {
            var host = EMBC.Tests.Integration.DRR.Application.Host;
            var applicationRepository = host.Services.GetRequiredService<IApplicationRepository>();

            var originalApplication = CreateTestEOIApplication();
            var id = (await applicationRepository.Manage(new SubmitApplication { Application = originalApplication })).Id;
            id.ShouldNotBeEmpty();

            var newApplication = (await applicationRepository.Query(new ApplicationsQuery { ApplicationId = id })).Items.ShouldHaveSingleItem();
            newApplication.ProjectTitle.ShouldNotBeEmpty();
            newApplication.Submitter.FirstName.ShouldBe(originalApplication.Submitter.FirstName);
            newApplication.ProjectContacts.Count().ShouldBe(originalApplication.ProjectContacts.Count());
        }

        private Application CreateTestEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            return new Application
            {
                ApplicantType = ApplicantType.LocalGovernment,
                ApplicantName = $"{uniqueSignature}_applicant_name",
                Submitter = CreateTestContact(uniqueSignature),
                ProjectContacts = new[]
                {
                    CreateTestContact(uniqueSignature)
                },
                ProjectTitle = $"{uniqueSignature}_projectTitle",
                ProjectType = ProjectType.New,
                RelatedHazards = new[]
                {
                    Hazards.Flood,
                    Hazards.Tsunami,
                    Hazards.Erosion,
                    Hazards.Other
                },
                OtherHazardsDescription = "Other Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                FundingRequest = 100,
                OtherFunding = new[]
                {
                    new FundingInformation
                    {
                        Name = "my $$$",
                        Amount = 100,
                        Type = FundingType.SelfFunding,
                    },
                    new FundingInformation
                    {
                        Name = "prov $$$",
                        Amount = 200,
                        Type = FundingType.Prov,
                    },
                },
                UnfundedAmount = 100,
                ReasonsToSecureFunding = "Funding Reasons",
                TotalFunding = 200,
                OwnershipDeclaration = true,
                LocationInformation = new LocationInformation
                {
                    Description = "location description",
                    Area = "123 acres",
                    Ownership = "owned"
                },
                BackgroundDescription = "background description",
                RationaleForFunding = "rationale for funding",
                ProposedSolution = "solution",
                RationaleForSolution = "rational for solution",
                EngagementProposal = "Engagement Proposal",
                ClimateAdaptation = "Climate Adaptation",
                OtherInformation = "Other Info",
                CFOConfirmation = true,
                FOIPPAConfirmation = true,
                IdentityConfirmation = true
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
                Department = "Position",
                Title = "Title"
            };
        }
    }


}
