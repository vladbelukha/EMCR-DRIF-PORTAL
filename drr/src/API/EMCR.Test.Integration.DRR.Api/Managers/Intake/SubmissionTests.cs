using AutoMapper;
using EMCR.DRR.API.Model;
using EMCR.DRR.Controllers;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EMCR.Tests.Integration.DRR.Managers.Intake
{
    public class SubmissionTests
    {
        private string TestPrefix = "autotest-dev";
        private string TestBusinessId = "autotest-dev-business-bceid";
        private string TestBusinessName = "autotest-dev-business-name";
        private string TestUserId = "autotest-dev-user-bceid";
        private UserInfo GetTestUserInfo()
        {
            return new UserInfo { BusinessId = TestBusinessId, BusinessName = TestBusinessName, UserId = TestUserId };
        }
        private readonly IIntakeManager manager;
        private readonly IMapper mapper;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public SubmissionTests()
        {
            var host = EMBC.Tests.Integration.DRR.Application.Host;
            manager = host.Services.GetRequiredService<IIntakeManager>();
            mapper = host.Services.GetRequiredService<IMapper>();
        }

        [Test]
        public async Task CanCreateEOIApplication()
        {
            var application = CreateNewTestEOIApplication();
            var id = await manager.Handle(new EoiSaveApplicationCommand { application = mapper.Map<EoiApplication>(application), UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();
        }

        [Test]
        public async Task CanSubmitEOIApplication()
        {
            var application = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            application.Status = SubmissionPortalStatus.UnderReview;
            application.AuthorizedRepresentativeStatement = true;
            application.FOIPPAConfirmation = true;
            application.InformationAccuracyStatement = true;

            var id = await manager.Handle(new EoiSubmitApplicationCommand { application = application, UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            var savedApplication = (await manager.Handle(new DrrApplicationsQuery { Id = id, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            savedApplication.Id.ShouldBe(id);
            savedApplication.AuthorizedRepresentativeStatement.ShouldBe(true);
            savedApplication.FOIPPAConfirmation.ShouldBe(true);
            savedApplication.InformationAccuracyStatement.ShouldBe(true);
            savedApplication.Status.ShouldBe(ApplicationStatus.Submitted);
            savedApplication.AdditionalContact1.ShouldNotBeNull();
            savedApplication.SubmittedDate.ShouldNotBeNull();
        }

        [Test]
        public async Task CanQueryApplications()
        {
            var application = CreateNewTestEOIApplication();
            var id = await manager.Handle(new EoiSaveApplicationCommand { application = mapper.Map<EoiApplication>(application), UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            var secondApplication = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            secondApplication.Status = SubmissionPortalStatus.UnderReview;
            secondApplication.AuthorizedRepresentativeStatement = true;
            secondApplication.FOIPPAConfirmation = true;
            secondApplication.InformationAccuracyStatement = true;

            var secondId = await manager.Handle(new EoiSubmitApplicationCommand { application = secondApplication, UserInfo = GetTestUserInfo() });
            secondId.ShouldNotBeEmpty();

            var thirdApplication = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            thirdApplication.Status = SubmissionPortalStatus.EligibleInvited;
            thirdApplication.AuthorizedRepresentativeStatement = true;
            thirdApplication.FOIPPAConfirmation = true;
            thirdApplication.InformationAccuracyStatement = true;

            var thirdId = await manager.Handle(new EoiSubmitApplicationCommand { application = thirdApplication, UserInfo = GetTestUserInfo() });
            thirdId.ShouldNotBeEmpty();

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = thirdId, UserInfo = GetTestUserInfo() });
            fpId.ShouldNotBeEmpty();

            var applications = (await manager.Handle(new DrrApplicationsQuery { BusinessId = GetTestUserInfo().BusinessId })).Items;
            var submissions = mapper.Map<IEnumerable<Submission>>(applications);
            submissions.ShouldContain(s => s.Id == id);
            submissions.ShouldContain(s => s.Id == secondId);
            submissions.ShouldContain(s => s.Id == thirdId);
            submissions.Single(s => s.Id == thirdId).ExistingFpId.ShouldNotBeNull();
        }

        [Test]
        public async Task CanCreateDraftEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            var application = new DraftEoiApplication
            {
                Status = SubmissionPortalStatus.Draft,
                //RelatedHazards = new[] { EMCR.DRR.Controllers.Hazards.Other },
            };
            var userInfo = new UserInfo
            {
                BusinessId = $"{uniqueSignature}_business-bceid",
                BusinessName = $"{uniqueSignature}_business-name",
                UserId = $"{uniqueSignature}_user-bceid"
            };
            var id = await manager.Handle(new EoiSaveApplicationCommand { application = mapper.Map<EoiApplication>(application), UserInfo = userInfo });
            id.ShouldNotBeEmpty();


            var savedApplication = (await manager.Handle(new DrrApplicationsQuery { Id = id, BusinessId = userInfo.BusinessId })).Items.SingleOrDefault();
            savedApplication.Id.ShouldBe(id);
            savedApplication.OwnershipDeclaration.ShouldBeNull();
            savedApplication.AuthorizedRepresentativeStatement.ShouldBe(false);
            savedApplication.SubmittedDate.ShouldBeNull();
        }

        [Test]
        public async Task SubmitMultipleApplications_SameSubmitter_MultipleSubmitterContactCreated()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            var application = CreateNewTestEOIApplication();
            application.Status = SubmissionPortalStatus.UnderReview;
            application.ProjectTitle = "First Submission";
            var id = await manager.Handle(new EoiSubmitApplicationCommand { application = mapper.Map<EoiApplication>(application), UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            var secondApplication = CreateNewTestEOIApplication();
            secondApplication.Status = SubmissionPortalStatus.UnderReview;
            secondApplication.ProjectTitle = "Second Submission";
            secondApplication.Submitter = application.Submitter;
            var secondId = await manager.Handle(new EoiSubmitApplicationCommand { application = mapper.Map<EoiApplication>(secondApplication), UserInfo = GetTestUserInfo() });
            secondId.ShouldNotBeEmpty();

            var host = EMBC.Tests.Integration.DRR.Application.Host;
            var drrCtxFactory = host.Services.GetRequiredService<IDRRContextFactory>();
            var ctx = drrCtxFactory.CreateReadOnly();

            var submitters = ctx.contacts.Where(c => c.drr_userid == TestUserId).ToList();
            submitters.Count.ShouldBeGreaterThan(1);
        }

        [Test]
        public async Task CanCreateFpFromEoi()
        {
            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = GetTestUserInfo() });
            eoiId.ShouldNotBeEmpty();

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetTestUserInfo() });
            fpId.ShouldNotBeEmpty();

            var fullProposal = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            fullProposal.Id.ShouldBe(fpId);
        }

        [Test]
        public async Task CanUpdateFp()
        {
            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = GetTestUserInfo() });
            eoiId.ShouldNotBeEmpty();

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetTestUserInfo() });
            fpId.ShouldNotBeEmpty();

            var fullProposal = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            fullProposal.Id.ShouldBe(fpId);

            var fpToUpdate = FillInFullProposal(mapper.Map<DraftFpApplication>(fullProposal));
            await manager.Handle(new FpSaveApplicationCommand { application = mapper.Map<FpApplication>(fpToUpdate), UserInfo = GetTestUserInfo() });

            var updatedFp = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            updatedFp.RegionalProject.ShouldBe(true);
            updatedFp.Standards.ShouldContain(s => s.Name == "Standard 1");
            updatedFp.Professionals.ShouldContain(p => p.Name == "professional1");
            updatedFp.LocalGovernmentEndorsement.ShouldBe(EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable);
        }

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        private DraftEoiApplication CreateNewTestEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            return new DraftEoiApplication
            {
                Status = SubmissionPortalStatus.Draft,

                //Proponent Information
                ProponentType = EMCR.DRR.Controllers.ProponentType.LocalGovernment,
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
                //InformationAccuracyStatement = true,
                //FOIPPAConfirmation = true,
                //AuthorizedRepresentativeStatement = true
            };
        }

        private DraftFpApplication FillInFullProposal(DraftFpApplication application)
        {
            application.RegionalProject = true;
            application.RegionalProjectComments = "regional comments";
            application.AuthorityAndOwnership = true;
            application.AuthorityAndOwnershipComments = "authority and ownership comments";
            application.OperationAndMaintenance = EMCR.DRR.Controllers.YesNoOption.Yes;
            application.OperationAndMaintenanceComments = "operation and maint. comments";
            application.FirstNationsEndorsement = EMCR.DRR.Controllers.YesNoOption.No;
            application.LocalGovernmentEndorsement = EMCR.DRR.Controllers.YesNoOption.NotApplicable;
            application.AuthorizationOrEndorsementComments = "authority or endorsement comments";
            application.Approvals = false;
            application.ApprovalsComments = "approvals comments";
            application.ProfessionalGuidance = false;
            application.Professionals = new[] { "professional1", "professional2" };
            application.ProfessionalGuidanceComments = "professional guidance comments";
            application.StandardsAcceptable = EMCR.DRR.Controllers.YesNoOption.NotApplicable;
            application.Standards = new[] { "Standard 1", "Standard 2" };
            application.StandardsComments = "professional guidance comments";
            application.Regulations = false;
            application.RegulationsComments = "regulations comments";

            return application;
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
