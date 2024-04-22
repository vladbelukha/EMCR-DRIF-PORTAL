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
                //Proponent Information
                ProponentType = EMCR.DRR.Controllers.ProponentType.LocalGovernment,
                ProponentName = $"{uniqueSignature}_applicant_name",
                Submitter = CreateNewTestContact(uniqueSignature),
                ProjectContact = CreateNewTestContact(uniqueSignature),
                AdditionalContacts = new[]
                {
                    CreateNewTestContact(uniqueSignature)
                },
                PartneringProponents = new[]
                {
                    "partner1",
                    "partner2"
                },

                //Project Information
                FundingStream = EMCR.DRR.Controllers.FundingStream.Stream1,
                ProjectTitle = $"{uniqueSignature}_projectTitle",
                ProjectType = EMCR.DRR.Controllers.ProjectType.New,
                ScopeStatement = "scope",
                RelatedHazards = new[]
                {
                    EMCR.DRR.Controllers.Hazards.Flood,
                    EMCR.DRR.Controllers.Hazards.Tsunami,
                    EMCR.DRR.Controllers.Hazards.Other
                },
                OtherHazardsDescription = "Other Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),

                //Funding Information
                EstimatedTotal = 1000,
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
                RemainingAmount = 600,
                IntendToSecureFunding = "Funding Reasons",

                //Location Information
                OwnershipDeclaration = true,
                OwnershipDescription = "owned",
                LocationDescription = "location description",

                //Project Detail
                RationaleForFunding = "rationale for funding",
                EstimatedPeopleImpacted = 5,
                CommunityImpact = "community impact",
                InfrastructureImpacted = new[] { "much infrastructure" },
                DisasterRiskUnderstanding = "helps many people",
                AdditionalBackgroundInformation = "additional background info",
                AddressRisksAndHazards = "fix risks",
                DRIFProgramGoalAlignment = "aligns with goals",
                AdditionalSolutionInformation = "additional solution info",
                RationaleForSolution = "rational for solution",

                //Engagement Plan
                FirstNationsEngagement = "Engagement Proposal",
                NeighbourEngagement = "engage with neighbours",
                AdditionalEngagementInformation = "additional engagement info",

                //Other Supporting Information
                ClimateAdaptation = "Climate Adaptation",
                OtherInformation = "Other Info",

                //Declaration
                FinancialAwarenessConfirmation = true,
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
