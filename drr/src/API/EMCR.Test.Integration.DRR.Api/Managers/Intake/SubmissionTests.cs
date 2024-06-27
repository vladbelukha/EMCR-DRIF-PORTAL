using EMCR.DRR.API.Model;
using EMCR.DRR.Controllers;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;

namespace EMCR.Tests.Integration.DRR.Managers.Intake
{
    public class SubmissionTests
    {
        private string TestPrefix = "autotest-dev";
        private string TestBusinessId = "autotest-dev-business-bceid";
        private string TestUserId = "autotest-dev-user-bceid";
        private readonly IIntakeManager manager;

        public SubmissionTests()
        {
            var host = EMBC.Tests.Integration.DRR.Application.Host;
            manager = host.Services.GetRequiredService<IIntakeManager>();
        }

        [Test]
        public async Task CanCreateEOIApplication()
        {
            var application = CreateNewTestEOIApplication();
            var id = await manager.Handle(new DrifEoiApplicationCommand { application = application, BusinessId = TestBusinessId, UserId = TestUserId });
            id.ShouldNotBeEmpty();
        }

        [Test]
        public async Task CanCreateDraftEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            var application = new DrifEoiApplication
            {
                Status = SubmissionPortalStatus.Draft,
                ProponentName = $"{uniqueSignature}_applicant_name",
                //RelatedHazards = new[] { EMCR.DRR.Controllers.Hazards.Other },
            };
            var id = await manager.Handle(new DrifEoiApplicationCommand { application = application, BusinessId = TestBusinessId, UserId = TestUserId });
            id.ShouldNotBeEmpty();
        }

        [Test]
        public async Task SubmitMultipleApplications_SameSubmitter_OnlyOneSubmitterContactCreated()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            var application = CreateNewTestEOIApplication();
            application.ProjectTitle = "First Submission";
            var id = await manager.Handle(new DrifEoiApplicationCommand { application = application, BusinessId = TestBusinessId, UserId = TestUserId });
            id.ShouldNotBeEmpty();

            var secondApplication = CreateNewTestEOIApplication();
            secondApplication.ProjectTitle = "Second Submission";
            var secondId = await manager.Handle(new DrifEoiApplicationCommand { application = secondApplication, BusinessId = TestBusinessId, UserId = TestUserId });
            secondId.ShouldNotBeEmpty();

            var host = EMBC.Tests.Integration.DRR.Application.Host;
            var drrCtxFactory = host.Services.GetRequiredService<IDRRContextFactory>();
            var ctx = drrCtxFactory.CreateReadOnly();

            var submitters = ctx.contacts.Where(c => c.drr_userid == TestUserId).ToList();
            submitters.ShouldHaveSingleItem();
        }

        private DrifEoiApplication CreateNewTestEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            return new DrifEoiApplication
            {
                Status = SubmissionPortalStatus.Draft,

                //Proponent Information
                ProponentType = EMCR.DRR.Controllers.ProponentType.LocalGovernment,
                ProponentName = $"{uniqueSignature}_applicant_name",
                Submitter = CreateNewTestContact(uniqueSignature, "submitter"),
                ProjectContact = CreateNewTestContact(uniqueSignature, "proj"),
                AdditionalContacts = new[]
                {
                    CreateNewTestContact(uniqueSignature, "add1"),
                    CreateNewTestContact(uniqueSignature, "add2"),
                },
                PartneringProponents = new[]
                {
                    $"{uniqueSignature}_partner1",
                    $"{uniqueSignature}_partner2"
                },

                //Project Information
                FundingStream = EMCR.DRR.Controllers.FundingStream.Stream1,
                ProjectTitle = "Project Title",
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
                        Name = $"{uniqueSignature}_Self",
                        Amount = 100,
                        Type = EMCR.DRR.Controllers.FundingType.SelfFunding,
                    },
                    new EMCR.DRR.Controllers.FundingInformation
                    {
                        Name = $"{uniqueSignature}_Prov",
                        Amount = 200,
                        Type = EMCR.DRR.Controllers.FundingType.Prov,
                    },
                    new EMCR.DRR.Controllers.FundingInformation
                    {
                        Name = $"{uniqueSignature}_Other",
                        Amount = 300,
                        Type = EMCR.DRR.Controllers.FundingType.OtherGrants,
                        OtherDescription = "other funding reason"
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
                EstimatedPeopleImpacted = EMCR.DRR.Controllers.EstimatedNumberOfPeople.OneToTenK,
                CommunityImpact = "community impact",
                InfrastructureImpacted = new[] { $"{uniqueSignature}_infrastructure1" },
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
                InformationAccuracyStatement = true,
                //FOIPPAConfirmation = true,
                AuthorizedRepresentativeStatement = true
            };
        }

        private EMCR.DRR.Controllers.ContactDetails CreateNewTestContact(string uniqueSignature, string namePrefix)
        {
            return new EMCR.DRR.Controllers.ContactDetails
            {
                FirstName = $"{uniqueSignature}_{namePrefix}_first",
                LastName = $"{uniqueSignature}_{namePrefix}_last",
                Email = "test@test.com",
                Phone = "604-123-4567",
                Department = "Position",
                Title = "Title"
            };
        }
    }
}
