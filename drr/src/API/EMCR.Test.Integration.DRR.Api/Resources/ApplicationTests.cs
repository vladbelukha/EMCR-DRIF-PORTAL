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

            var newApplication = (await applicationRepository.Query(new ApplicationsQuery { ApplicationName = id })).Items.ShouldHaveSingleItem();
            newApplication.ProjectTitle.ShouldNotBeEmpty();
            //verify submitter
            //verify project contact
            //verify additonal contacts
            //verify partnering proponents
            //verify funding sourcres
            //verify infrastructure
        }

        [Test]
        public async Task CanQueryDeclarations()
        {
            var host = EMBC.Tests.Integration.DRR.Application.Host;
            var applicationRepository = host.Services.GetRequiredService<IApplicationRepository>();

            var declarations = (await applicationRepository.Query(new EMCR.DRR.Resources.Applications.DeclarationQuery { })).Items;
            declarations.ShouldNotBeEmpty();
        }

        [Test]
        public async Task CanQueryApplications()
        {
            var host = EMBC.Tests.Integration.DRR.Application.Host;
            var applicationRepository = host.Services.GetRequiredService<IApplicationRepository>();

            var applications = (await applicationRepository.Query(new ApplicationsQuery { })).Items;
            applications.ShouldNotBeEmpty();
            //newApplication.Submitter.FirstName.ShouldBe(originalApplication.Submitter.FirstName);
            //newApplication.AdditionalContacts.Count().ShouldBe(originalApplication.AdditionalContacts.Count());
        }

        private Application CreateTestEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            return new Application
            {
                //Proponent Information
                ProponentType = ProponentType.LocalGovernment,
                ProponentName = $"{uniqueSignature}_applicant_name",
                Submitter = CreateTestContact(uniqueSignature, "submitter"),
                ProjectContact = CreateTestContact(uniqueSignature, "proj"),
                AdditionalContact1 = CreateTestContact(uniqueSignature, "add1"),
                //AdditionalContact2 = CreateTestContact(uniqueSignature),
                PartneringProponents = new[]
                {
                   new PartneringProponent { Name = $"{uniqueSignature}_partner1" },
                   new PartneringProponent { Name = $"{uniqueSignature}_partner2" },
                },

                //Project Information
                FundingStream = FundingStream.Stream1,
                ProjectTitle = "Project Title",
                ProjectType = ProjectType.New,
                ScopeStatement = "scope",
                RelatedHazards = new[]
                {
                    Hazards.Flood,
                    Hazards.Tsunami,
                    Hazards.Other
                },
                OtherHazardsDescription = "Other Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),

                //Funding Information
                EstimatedTotal = 1000,
                FundingRequest = 100,
                OtherFunding = new[]
                {
                    new FundingInformation
                    {
                        Name = $"{uniqueSignature}_self",
                        Amount = 100,
                        Type = FundingType.SelfFunding,
                    },
                    new FundingInformation
                    {
                        Name = $"{uniqueSignature}_prov",
                        Amount = 200,
                        Type = FundingType.Prov,
                    },
                    new FundingInformation
                    {
                        Name = $"{uniqueSignature}_other",
                        Amount = 300,
                        Type = FundingType.Other,
                        OtherDescription = "reason for other funding"
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
                EstimatedPeopleImpacted = EstimatedNumberOfPeople.OneToTenK,
                CommunityImpact = "community impact",
                InfrastructureImpacted = new[]
                {
                    new CriticalInfrastructure {Name= $"{uniqueSignature}_infrastructure1" },
                    new CriticalInfrastructure {Name= $"{uniqueSignature}_infrastructure2" },
                },
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
                FOIPPAConfirmation = true,
                AuthorizedRepresentativeStatement = true
            };
        }

        private ContactDetails CreateTestContact(string uniqueSignature, string namePrefix)
        {
            return new ContactDetails
            {
                FirstName = $"{uniqueSignature}_{namePrefix}_first",
                LastName = $"{uniqueSignature}_{namePrefix}_last",
                Email = "test@test.com",
                Phone = "6041234567",
                Department = "Position",
                Title = "Title"
            };
        }
    }


}
