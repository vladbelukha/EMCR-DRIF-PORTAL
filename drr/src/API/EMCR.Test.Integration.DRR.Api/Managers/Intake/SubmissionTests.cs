using EMCR.DRR.Controllers;
using EMCR.DRR.Managers.Intake;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EMCR.Tests.Integration.DRR.Managers.Intake
{
    public class SubmissionTests
    {
        private string TestPrefix = "autotest-dev";
        private readonly IIntakeManager manager;

        public SubmissionTests()
        {
            var host = EMBC.Tests.Integration.DRR.Application.Host;
            manager = host.Services.GetRequiredService<IIntakeManager>();
        }

        [Test]
        public async Task CanSubmitEOIApplication()
        {
            var application = CreateNewTestEOIApplication();
            var id = await manager.Handle(new DrifEoiApplicationCommand { application = application });
            id.ShouldNotBeEmpty();
        }

        private DrifEoiApplication CreateNewTestEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            return new DrifEoiApplication
            {
                ApplicantType = EMCR.DRR.Controllers.ApplicantType.LocalGovernment,
                ApplicantName = $"{uniqueSignature}_applicant_name",
                Submitter = CreateNewTestContact(uniqueSignature),
                ProjectContacts = new[]
                {
                    CreateNewTestContact(uniqueSignature)
                },
                ProjectTitle = $"{uniqueSignature}_projectTitle",
                ProjectType = EMCR.DRR.Controllers.ProjectType.New,
                RelatedHazards = new[]
                {
                    EMCR.DRR.Controllers.Hazards.Flood,
                    EMCR.DRR.Controllers.Hazards.Tsunami,
                    EMCR.DRR.Controllers.Hazards.Erosion,
                    EMCR.DRR.Controllers.Hazards.Other
                },
                OtherHazardsDescription = "Other Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                FundingRequest = 100,
                OtherFunding = new[]
                {
                    new EMCR.DRR.Controllers.FundingInformation
                    {
                        Name = "my $$$",
                        Amount = 100,
                        Type = EMCR.DRR.Controllers.FundingType.SelfFunding,
                    },
                    new EMCR.DRR.Controllers.FundingInformation
                    {
                        Name = "prov $$$",
                        Amount = 200,
                        Type = EMCR.DRR.Controllers.FundingType.Prov,
                    },
                },
                UnfundedAmount = 100,
                ReasonsToSecureFunding = "Funding Reasons",
                TotalFunding = 200,
                OwnershipDeclaration = true,
                LocationInformation = new EMCR.DRR.Controllers.LocationInformation
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

        private EMCR.DRR.Controllers.ContactDetails CreateNewTestContact(string uniqueSignature)
        {
            return new EMCR.DRR.Controllers.ContactDetails
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
