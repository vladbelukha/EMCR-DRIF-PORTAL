using System.Text;
using AutoMapper;
using EMCR.DRR.API.Model;
using EMCR.DRR.API.Services.S3;
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8629 // Nullable value type may be null.
        public SubmissionTests()
        {
            var host = Application.Host;
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
            savedApplication.InfrastructureImpacted.ShouldHaveSingleItem().Impact.ShouldNotBeNullOrEmpty();
        }

        [Test]
        public async Task CanSubmitFPApplication()
        {
            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = GetTestUserInfo() });
            eoiId.ShouldNotBeEmpty();

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetTestUserInfo(), ScreenerQuestions = CreateScreenerQuestions() });
            fpId.ShouldNotBeEmpty();

            var fullProposal = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            fullProposal.Id.ShouldBe(fpId);
            fullProposal.EoiId.ShouldBe(eoiId);

            var fpToUpdate = FillInFullProposal(mapper.Map<DraftFpApplication>(fullProposal));
            fpToUpdate.Submitter = eoi.Submitter;
            await manager.Handle(new FpSubmitApplicationCommand { application = mapper.Map<FpApplication>(fpToUpdate), UserInfo = GetTestUserInfo() });

            var updatedFp = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            updatedFp.Status.ShouldBe(ApplicationStatus.Submitted);
        }

        [Test]
        public async Task QueryApplications_CanGetSpecificPage()
        {
            var queryOptions = new QueryOptions { Page = 2, PageSize = 15 };
            //var queryOptions = new QueryOptions { Page = 1, PageSize = 100 };
            var queryRes = await manager.Handle(new DrrApplicationsQuery { BusinessId = GetTestUserInfo().BusinessId, QueryOptions = queryOptions });
            var applications = queryRes.Items;
            var submissions = mapper.Map<IEnumerable<Submission>>(applications);
            submissions.Count().ShouldBeLessThanOrEqualTo(15);
        }

        [Test]
        public async Task QueryApplications_CanFilterByField()
        {
            var queryOptions = new QueryOptions { Filter = "programType=DRIF,applicationType=FP,status=*UnderReview\\|EligiblePending" };
            var queryRes = await manager.Handle(new DrrApplicationsQuery { BusinessId = GetTestUserInfo().BusinessId, QueryOptions = queryOptions });
            var applications = queryRes.Items;
            var submissions = mapper.Map<IEnumerable<Submission>>(applications);
            //submissions.Count().ShouldBe(20);
            submissions.ShouldAllBe(s => s.ProgramType == ProgramType.DRIF);
            submissions.ShouldAllBe(s => s.ApplicationType == ApplicationType.FP);
            submissions.ShouldAllBe(s => s.Status == SubmissionPortalStatus.UnderReview || s.Status == SubmissionPortalStatus.EligiblePending);
        }

        [Test]
        public async Task QueryApplications_CanSortResults()
        {
            var queryOptions = new QueryOptions { OrderBy = "status desc" };
            var queryRes = await manager.Handle(new DrrApplicationsQuery { BusinessId = GetTestUserInfo().BusinessId, QueryOptions = queryOptions });
            var applications = queryRes.Items;
            var submissions = mapper.Map<IEnumerable<Submission>>(applications);
            submissions.Count().ShouldBeLessThanOrEqualTo(20);
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

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = thirdId, UserInfo = GetTestUserInfo(), ScreenerQuestions = CreateScreenerQuestions() });
            fpId.ShouldNotBeEmpty();

            var queryRes = await manager.Handle(new DrrApplicationsQuery { BusinessId = GetTestUserInfo().BusinessId });
            var applications = queryRes.Items;
            var submissions = mapper.Map<IEnumerable<Submission>>(applications);
            submissions.ShouldContain(s => s.Id == id);
            submissions.ShouldContain(s => s.Id == secondId);
            submissions.ShouldContain(s => s.Id == thirdId);
            submissions.Single(s => s.Id == thirdId).ExistingFpId.ShouldNotBeNull();
            submissions.ShouldAllBe(s => s.ProgramType == ProgramType.DRIF);
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
        public async Task SaveEOI_BlankInfrastructure_BlankRecordNotCreated()
        {
            var application = CreateNewTestEOIApplication();

            application.InfrastructureImpacted = new[]
            {
                new InfrastructureImpacted
                {
                    Infrastructure = string.Empty,
                    Impact = string.Empty,
                },
                    null
            };

            var id = await manager.Handle(new EoiSaveApplicationCommand { application = mapper.Map<EoiApplication>(application), UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            var savedApplication = (await manager.Handle(new DrrApplicationsQuery { Id = id, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            savedApplication.InfrastructureImpacted.Count().ShouldBe(0);
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

            var host = Application.Host;
            var drrCtxFactory = host.Services.GetRequiredService<IDRRContextFactory>();
            var ctx = drrCtxFactory.CreateReadOnly();

            var submitters = ctx.contacts.Where(c => c.drr_userid == TestUserId).ToList();
            submitters.Count.ShouldBeGreaterThan(1);
        }

        [Test]
        public async Task CanCreateFpFromEoi()
        {
            var userInfo = GetTestUserInfo();
            //var userInfo = GetCRAFTUserInfo();

            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = userInfo });
            eoiId.ShouldNotBeEmpty();

            var screenerQuestions = CreateScreenerQuestions();
            screenerQuestions.FirstNationsAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.LocalGovernmentAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.EngagedWithFirstNationsOccurred = false;

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = userInfo, ScreenerQuestions = screenerQuestions });
            fpId.ShouldNotBeEmpty();

            var fullProposal = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = userInfo.BusinessId })).Items.SingleOrDefault();
            fullProposal.Id.ShouldBe(fpId);

            fullProposal.RegionalProject.ShouldBeNull();
            fullProposal.MainDeliverable.ShouldBe(eoi.ScopeStatement);
            fullProposal.HaveAuthorityToDevelop.ShouldBe(screenerQuestions.HaveAuthorityToDevelop);
            fullProposal.FirstNationsAuthorizedByPartners.ShouldBe(screenerQuestions.FirstNationsAuthorizedByPartners);
            fullProposal.LocalGovernmentAuthorizedByPartners.ShouldBe(screenerQuestions.LocalGovernmentAuthorizedByPartners);
            //fullProposal.FoundationWorkCompleted.ShouldBe(screenerQuestions.FoundationWorkCompleted);
            fullProposal.EngagedWithFirstNationsOccurred.ShouldBeNull();
            fullProposal.IncorporateFutureClimateConditions.ShouldBe(screenerQuestions.IncorporateFutureClimateConditions);
            fullProposal.MeetsRegulatoryRequirements.ShouldBe(screenerQuestions.MeetsRegulatoryRequirements);
            fullProposal.MeetsEligibilityRequirements.ShouldBe(screenerQuestions.MeetsEligibilityRequirements);
            fullProposal.InfrastructureImpacted.ShouldHaveSingleItem();
        }

        [Test]
        public async Task DeleteFP_ThrowsError()
        {
            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = GetTestUserInfo() });
            eoiId.ShouldNotBeEmpty();

            var screenerQuestions = CreateScreenerQuestions();
            screenerQuestions.FirstNationsAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.LocalGovernmentAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.EngagedWithFirstNationsOccurred = false;

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetTestUserInfo(), ScreenerQuestions = screenerQuestions });
            fpId.ShouldNotBeEmpty();

            var fullProposal = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            fullProposal.Id.ShouldBe(fpId);

            Should.Throw<Exception>(() => manager.Handle(new DeleteApplicationCommand { Id = fpId, UserInfo = GetTestUserInfo() }));
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

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetTestUserInfo(), ScreenerQuestions = CreateScreenerQuestions() });
            fpId.ShouldNotBeEmpty();

            var fullProposal = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            fullProposal.Id.ShouldBe(fpId);
            fullProposal.EoiId.ShouldBe(eoiId);
            fullProposal.HowWasNeedIdentified.ShouldBe(eoi.RationaleForSolution);

            var fpToUpdate = FillInFullProposal(mapper.Map<DraftFpApplication>(fullProposal));
            await manager.Handle(new FpSaveApplicationCommand { application = mapper.Map<FpApplication>(fpToUpdate), UserInfo = GetTestUserInfo() });

            var updatedFp = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            updatedFp.RegionalProject.ShouldBe(true);
            updatedFp.IsInfrastructureImpacted.ShouldBe(true);
            updatedFp.EstimatedPeopleImpactedFP.ShouldBe(EMCR.DRR.Managers.Intake.EstimatedNumberOfPeopleFP.FiveHundredToOneK);
            updatedFp.Standards.ShouldContain(s => s.Category == "Other");
            updatedFp.Standards.Single(s => s.Category == "Other").IsCategorySelected.ShouldBe(true);
            updatedFp.Professionals.ShouldContain(p => p.Name == "professional1");
            updatedFp.LocalGovernmentAuthorizedByPartners.ShouldBe(EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable);
            ((int)updatedFp.OperationAndMaintenance).ShouldBe((int)fpToUpdate.OperationAndMaintenance);
            updatedFp.ClimateAssessmentTools.ShouldNotBeEmpty();
            updatedFp.ClimateAssessmentComments.ShouldBe("climate assessment comments");
            updatedFp.IncreasedOrTransferred.ShouldNotBeEmpty();
            updatedFp.IntendToSecureFunding.ShouldBe(fpToUpdate.IntendToSecureFunding);

            var ret = mapper.Map<DraftFpApplication>(updatedFp);
            ret.FoundationalOrPreviousWorks.ShouldContain("autotest-verification-method");
            ret.AffectedParties.ShouldContain("party 1");
            ret.ClimateAssessmentTools.ShouldContain("tool 1");
            ret.Professionals.ShouldContain("professional1");
            ret.CostReductions.ShouldContain("cost reduction 1");
            ret.CoBenefits.ShouldContain("benefit 1");
            ret.IncreasedResiliency.ShouldContain("resiliency 1");
            ret.ComplexityRisks.ShouldContain("complexity risk 1");
            ret.ReadinessRisks.ShouldContain("readiness risk 1");
            ret.SensitivityRisks.ShouldContain("sensitivity risk 1");
            ret.CapacityRisks.ShouldContain("capacity risk 1");
            ret.CostConsiderations.ShouldContain("cost consideration 1");
            ret.PreviousResponseComments.ShouldBe(fpToUpdate.PreviousResponseComments);
            ret.MeetsEligibilityRequirements.ShouldBe(fpToUpdate.MeetsEligibilityRequirements);
            ret.MeetsEligibilityComments.ShouldBe(fpToUpdate.MeetsEligibilityComments);
            ret.TotalProjectCost.ShouldBe(fpToUpdate.TotalProjectCost);
            ret.HowWasNeedIdentified.ShouldBe(fpToUpdate.HowWasNeedIdentified);
        }

        [Test]
        public async Task UpdateFP_ListsUpdateCorrectly()
        {
            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = GetTestUserInfo() });
            eoiId.ShouldNotBeEmpty();

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetTestUserInfo(), ScreenerQuestions = CreateScreenerQuestions() });
            fpId.ShouldNotBeEmpty();

            var fullProposal = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            fullProposal.Id.ShouldBe(fpId);
            fullProposal.EoiId.ShouldBe(eoiId);

            var fpToUpdate = FillInFullProposal(mapper.Map<DraftFpApplication>(fullProposal));
            await manager.Handle(new FpSaveApplicationCommand { application = mapper.Map<FpApplication>(fpToUpdate), UserInfo = GetTestUserInfo() });

            var updatedFp = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            updatedFp.RegionalProject.ShouldBe(true);
            updatedFp.Standards.ShouldContain(s => s.Category == "Other");
            updatedFp.Professionals.ShouldContain(p => p.Name == "professional1");
            updatedFp.LocalGovernmentAuthorizedByPartners.ShouldBe(EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable);

            fpToUpdate = FillInFullProposal(mapper.Map<DraftFpApplication>(updatedFp));

            await manager.Handle(new FpSaveApplicationCommand { application = mapper.Map<FpApplication>(fpToUpdate), UserInfo = GetTestUserInfo() });

            var twiceUpdatedFp = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();

            twiceUpdatedFp.Professionals.Count().ShouldBe(fpToUpdate.Professionals.Count());
            twiceUpdatedFp.Standards.Count().ShouldBe(6);
            twiceUpdatedFp.ProposedActivities.Count().ShouldBe(fpToUpdate.ProposedActivities.Count());
            twiceUpdatedFp.FoundationalOrPreviousWorks.Count().ShouldBe(fpToUpdate.FoundationalOrPreviousWorks.Count());
            twiceUpdatedFp.AffectedParties.Count().ShouldBe(fpToUpdate.AffectedParties.Count());
            twiceUpdatedFp.CostReductions.Count().ShouldBe(fpToUpdate.CostReductions.Count());
            twiceUpdatedFp.CoBenefits.Count().ShouldBe(fpToUpdate.CoBenefits.Count());
            twiceUpdatedFp.IncreasedResiliency.Count().ShouldBe(fpToUpdate.IncreasedResiliency.Count());
            twiceUpdatedFp.ComplexityRisks.Count().ShouldBe(fpToUpdate.ComplexityRisks.Count());
            twiceUpdatedFp.ReadinessRisks.Count().ShouldBe(fpToUpdate.ReadinessRisks.Count());
            twiceUpdatedFp.SensitivityRisks.Count().ShouldBe(fpToUpdate.SensitivityRisks.Count());
            twiceUpdatedFp.CapacityRisks.Count().ShouldBe(fpToUpdate.CapacityRisks.Count());
            ////twiceUpdatedFp.TransferRisks.Count().ShouldBe(fpToUpdate.TransferRisks.Count());
            twiceUpdatedFp.YearOverYearFunding.Count().ShouldBe(fpToUpdate.YearOverYearFunding.Count());
            //twiceUpdatedFp.CostConsiderations.Count().ShouldBe(fpToUpdate.CostConsiderations.Count());
        }

        [Test]
        public async Task WithdrawApplication_StatusDraft_ThrowsError()
        {
            var application = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            application.Status = SubmissionPortalStatus.Draft;

            var id = await manager.Handle(new EoiSaveApplicationCommand { application = application, UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            Should.Throw<Exception>(() => manager.Handle(new WithdrawApplicationCommand { Id = id, UserInfo = GetTestUserInfo() }));
        }

        [Test]
        public async Task WithdrawApplication_StatusSubmitted_ApplicationWithdrawn()
        {
            var application = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            application.Status = SubmissionPortalStatus.UnderReview;
            application.AuthorizedRepresentativeStatement = true;
            application.FOIPPAConfirmation = true;
            application.InformationAccuracyStatement = true;

            var id = await manager.Handle(new EoiSubmitApplicationCommand { application = application, UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            await manager.Handle(new WithdrawApplicationCommand { Id = id, UserInfo = GetTestUserInfo() });
            var withdrawnApplication = (await manager.Handle(new DrrApplicationsQuery { Id = id, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            withdrawnApplication.Status.ShouldBe(ApplicationStatus.Withdrawn);
        }

        [Test]
        public async Task DeleteApplication_StatusSubmitted_ThrowsError()
        {
            var application = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            application.Status = SubmissionPortalStatus.UnderReview;
            application.AuthorizedRepresentativeStatement = true;
            application.FOIPPAConfirmation = true;
            application.InformationAccuracyStatement = true;

            var id = await manager.Handle(new EoiSaveApplicationCommand { application = application, UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            Should.Throw<Exception>(() => manager.Handle(new DeleteApplicationCommand { Id = id, UserInfo = GetTestUserInfo() }));
        }

        [Test]
        public async Task DeleteApplication_StatusDraft_StatusUpdatedToDeleted()
        {
            var application = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            application.Status = SubmissionPortalStatus.Draft;

            var id = await manager.Handle(new EoiSaveApplicationCommand { application = application, UserInfo = GetTestUserInfo() });
            id.ShouldNotBeEmpty();

            var res = await manager.Handle(new DeleteApplicationCommand { Id = id, UserInfo = GetTestUserInfo() });
            res.ShouldBe(id);

            var deletedApplication = (await manager.Handle(new DrrApplicationsQuery { Id = id, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            deletedApplication.ShouldBeNull();
        }

        [Test]
        public async Task CanAddAttachment()
        {
            var userInfo = GetTestUserInfo();
            //var userInfo = GetCRAFTUserInfo();

            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = userInfo });
            eoiId.ShouldNotBeEmpty();

            var screenerQuestions = CreateScreenerQuestions();
            screenerQuestions.FirstNationsAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.LocalGovernmentAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.EngagedWithFirstNationsOccurred = false;

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = userInfo, ScreenerQuestions = screenerQuestions });
            fpId.ShouldNotBeEmpty();

            var body = DateTime.Now.ToString();
            var fileName = "autotest.txt";
            byte[] bytes = Encoding.ASCII.GetBytes(body);
            var file = new S3File { FileName = fileName, Content = bytes, ContentType = "text/plain", };

            var documentId = await manager.Handle(new UploadAttachmentCommand { AttachmentInfo = new AttachmentInfo { ApplicationId = fpId, File = file, DocumentType = EMCR.DRR.Managers.Intake.DocumentType.SitePlan }, UserInfo = GetTestUserInfo() });
            var fullProposal = mapper.Map<DraftFpApplication>((await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = userInfo.BusinessId })).Items.SingleOrDefault());
            fullProposal.HaveResolution = true;
            fullProposal.Attachments.Count().ShouldBe(1);
            fullProposal.Attachments.First().DocumentType.ShouldBe(EMCR.DRR.API.Model.DocumentType.SitePlan);
            fullProposal.Attachments.First().Comments = "site plan comments";

            await manager.Handle(new FpSaveApplicationCommand { application = mapper.Map<FpApplication>(fullProposal), UserInfo = GetTestUserInfo() });

            var updatedFp = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            updatedFp.Attachments.First().Comments.ShouldBe("site plan comments");
            updatedFp.HaveResolution.ShouldBe(true);
        }

        [Test]
        public async Task CanDeleteAttachment()
        {
            var userInfo = GetTestUserInfo();
            //var userInfo = GetCRAFTUserInfo();

            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = userInfo });
            eoiId.ShouldNotBeEmpty();

            var screenerQuestions = CreateScreenerQuestions();
            screenerQuestions.FirstNationsAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.LocalGovernmentAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.EngagedWithFirstNationsOccurred = false;

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = userInfo, ScreenerQuestions = screenerQuestions });
            fpId.ShouldNotBeEmpty();

            var body = DateTime.Now.ToString();
            var fileName = "autotest.txt";
            byte[] bytes = Encoding.ASCII.GetBytes(body);
            var file = new S3File { FileName = fileName, Content = bytes, ContentType = "text/plain", };

            var documentId = await manager.Handle(new UploadAttachmentCommand { AttachmentInfo = new AttachmentInfo { ApplicationId = fpId, File = file, DocumentType = EMCR.DRR.Managers.Intake.DocumentType.SitePlan }, UserInfo = GetTestUserInfo() });
            var fullProposal = mapper.Map<DraftFpApplication>((await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = userInfo.BusinessId })).Items.SingleOrDefault());
            fullProposal.Attachments.Count().ShouldBe(1);

            await manager.Handle(new DeleteAttachmentCommand { Id = documentId, UserInfo = GetTestUserInfo() });

            var updatedFp = (await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = GetTestUserInfo().BusinessId })).Items.SingleOrDefault();
            updatedFp.Attachments.Count().ShouldBe(0);
        }

        [Test]
        public async Task AddAttachment_NoDocumentType_DefaultsToOther()
        {
            var userInfo = GetTestUserInfo();
            //var userInfo = GetCRAFTUserInfo();

            var eoi = mapper.Map<EoiApplication>(CreateNewTestEOIApplication());
            eoi.Status = SubmissionPortalStatus.EligibleInvited;
            eoi.AuthorizedRepresentativeStatement = true;
            eoi.FOIPPAConfirmation = true;
            eoi.InformationAccuracyStatement = true;

            var eoiId = await manager.Handle(new EoiSubmitApplicationCommand { application = eoi, UserInfo = userInfo });
            eoiId.ShouldNotBeEmpty();

            var screenerQuestions = CreateScreenerQuestions();
            screenerQuestions.FirstNationsAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.LocalGovernmentAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.NotApplicable;
            screenerQuestions.EngagedWithFirstNationsOccurred = false;

            var fpId = await manager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = userInfo, ScreenerQuestions = screenerQuestions });
            fpId.ShouldNotBeEmpty();

            var body = DateTime.Now.ToString();
            var fileName = "autotest.txt";
            byte[] bytes = Encoding.ASCII.GetBytes(body);
            var file = new S3File { FileName = fileName, Content = bytes, ContentType = "text/plain", };

            var documentId = await manager.Handle(new UploadAttachmentCommand { AttachmentInfo = new AttachmentInfo { ApplicationId = fpId, File = file }, UserInfo = GetTestUserInfo() });
            var fullProposal = mapper.Map<DraftFpApplication>((await manager.Handle(new DrrApplicationsQuery { Id = fpId, BusinessId = userInfo.BusinessId })).Items.SingleOrDefault());
            fullProposal.HaveResolution = true;
            fullProposal.Attachments.Count().ShouldBe(1);
            fullProposal.Attachments.First().DocumentType.ShouldBe(EMCR.DRR.API.Model.DocumentType.OtherSupportingDocument);
        }

#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8629 // Nullable value type may be null.
        private EMCR.DRR.Managers.Intake.ScreenerQuestions CreateScreenerQuestions()
        {
            return new EMCR.DRR.Managers.Intake.ScreenerQuestions
            {
                ProjectWorkplan = true,
                ProjectSchedule = true,
                CostEstimate = true,
                SitePlan = EMCR.DRR.Managers.Intake.YesNoOption.Yes,
                HaveAuthorityToDevelop = true,
                FirstNationsAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.Yes,
                LocalGovernmentAuthorizedByPartners = EMCR.DRR.Managers.Intake.YesNoOption.Yes,
                FoundationWorkCompleted = EMCR.DRR.Managers.Intake.YesNoOption.Yes,
                EngagedWithFirstNationsOccurred = true,
                IncorporateFutureClimateConditions = true,
                MeetsRegulatoryRequirements = true,
                MeetsEligibilityRequirements = true,
            };
        }

        private DraftEoiApplication CreateNewTestEOIApplication()
        {
            var uniqueSignature = TestPrefix + "-" + Guid.NewGuid().ToString().Substring(0, 4);
            return new DraftEoiApplication
            {
                Status = SubmissionPortalStatus.Draft,

                //Proponent Information - 1
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

                //Project Information - 2
                FundingStream = EMCR.DRR.Controllers.FundingStream.Stream1,
                ProjectTitle = "Project Title",
                Stream = EMCR.DRR.Controllers.ProjectType.New,
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

                //Funding Information - 3
                EstimatedTotal = 1000,
                FundingRequest = 100,
                HaveOtherFunding = true,
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
                RemainingAmount = 300,
                IntendToSecureFunding = "Funding Reasons",

                //Location Information - 4
                OwnershipDeclaration = true,
                OwnershipDescription = "owned",
                LocationDescription = "location description",

                //Project Detail - 5
                RationaleForFunding = "rationale for funding",
                EstimatedPeopleImpacted = EMCR.DRR.Controllers.EstimatedNumberOfPeople.OneToTenK,
                CommunityImpact = "community impact",
                IsInfrastructureImpacted = true,
                InfrastructureImpacted = new[] { new InfrastructureImpacted { Infrastructure = $"{uniqueSignature}_infrastructure1", Impact = "impact" } },
                DisasterRiskUnderstanding = "helps many people",
                AdditionalBackgroundInformation = "additional background info",
                AddressRisksAndHazards = "fix risks",
                ProjectDescription = "project description",
                AdditionalSolutionInformation = "additional solution info",
                RationaleForSolution = "rational for solution",

                //Engagement Plan - 6
                FirstNationsEngagement = "Engagement Proposal",
                NeighbourEngagement = "engage with neighbours",
                AdditionalEngagementInformation = "additional engagement info",

                //Other Supporting Information - 7
                ClimateAdaptation = "Climate Adaptation",
                OtherInformation = "Other Info",

                //Declaration - 8
                //InformationAccuracyStatement = true,
                //FOIPPAConfirmation = true,
                //AuthorizedRepresentativeStatement = true
            };
        }

        private DraftFpApplication FillInFullProposal(DraftFpApplication application)
        {
            //Proponent & Project Information - 1
            application.RegionalProject = true;
            application.RegionalProjectComments = "regional comments";
            application.MainDeliverable = "main deliverable";

            //Ownership & Authorization - 2
            application.HaveAuthorityToDevelop = true;
            application.OperationAndMaintenance = EMCR.DRR.Controllers.YesNoOption.Yes;
            application.OperationAndMaintenanceComments = "operation and maint. comments";
            application.FirstNationsAuthorizedByPartners = EMCR.DRR.Controllers.YesNoOption.No;
            application.LocalGovernmentAuthorizedByPartners = EMCR.DRR.Controllers.YesNoOption.NotApplicable;
            application.AuthorizationOrEndorsementComments = "authority or endorsement comments";

            //Project Area - 3
            application.Area = 123;
            application.Units = EMCR.DRR.Controllers.AreaUnits.Acres;
            application.AreaDescription = "area description";
            application.IsInfrastructureImpacted = true;
            application.EstimatedPeopleImpactedFP = EMCR.DRR.Controllers.EstimatedNumberOfPeopleFP.FiveHundredToOneK;

            //Project Plan - 4
            application.ProposedActivities = new[]
            {
                new EMCR.DRR.Controllers.ProposedActivity {StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), Name = "autotest-proposed-activity-name", RelatedMilestone = "some milestone" }
            };
            application.FoundationalOrPreviousWorks = new[] { "autotest-verification-method" };
            application.HowWasNeedIdentified = "need identified";
            application.ProjectAlternateOptions = "some alternate options";

            //Project Engagement - 5
            application.EngagedWithFirstNationsComments = "first nations comments";
            application.OtherEngagement = EMCR.DRR.Controllers.YesNoOption.Yes;
            application.AffectedParties = new[] { "party 1", "party 2" };
            application.OtherEngagementComments = "other engagement comments";
            application.CollaborationComments = "collaboration comments";

            //Climate Adaptation - 6
            application.IncorporateFutureClimateConditions = true;
            application.ClimateAssessment = true;
            application.ClimateAssessmentTools = new[] { "tool 1", "tool 2" };
            application.ClimateAssessmentComments = "climate assessment comments";

            //Permits Regulations & Standards - 7
            application.StandardsAcceptable = EMCR.DRR.Controllers.YesNoOption.NotApplicable;
            application.Standards = new[] {
                new EMCR.DRR.Controllers.StandardInfo { IsCategorySelected = true, Category = "Environment - Water (includes Rivers, Flooding, etc.)", Standards = new [] { "BC Water Sustainability Act", "Water Survey Canada", "other water env standard" } },
                new EMCR.DRR.Controllers.StandardInfo { IsCategorySelected = true, Category = "Other", Standards = new [] { "other_standard1"} },
            };
            application.StandardsComments = "standards comments";
            application.ProfessionalGuidance = false;
            application.Professionals = new[] { "professional1", "professional2" };
            application.ProfessionalGuidanceComments = "professional guidance comments";
            application.MeetsRegulatoryRequirements = false;
            application.MeetsRegulatoryComments = "regulations comments";
            application.MeetsEligibilityRequirements = false;
            application.MeetsEligibilityComments = "eligibility comments";

            //Project Outcomes - 8
            application.PublicBenefit = false;
            application.PublicBenefitComments = "public benefit comments";
            application.FutureCostReduction = true;
            application.CostReductions = new[] { "cost reduction 1", "cost reduction 2" };
            application.CostReductionComments = "cost reduction comments";
            application.ProduceCoBenefits = true;
            application.CoBenefits = new[] { "benefit 1", "benefit 2" };
            application.CoBenefitComments = "benefit comments";
            application.IncreasedResiliency = new[] { "resiliency 1", "resiliency 2" };
            application.IncreasedResiliencyComments = "resiliency comments";

            //Project Risks - 9
            application.ComplexityRiskMitigated = true;
            application.ComplexityRisks = new[] { "complexity risk 1", "complexity risk 2" };
            application.ComplexityRiskComments = "risk comments";
            application.ReadinessRiskMitigated = true;
            application.ReadinessRisks = new[] { "readiness risk 1", "readiness risk 2" };
            application.ReadinessRiskComments = "readiness comments";
            application.SensitivityRiskMitigated = true;
            application.SensitivityRisks = new[] { "sensitivity risk 1", "sensitivity risk 2" };
            application.SensitivityRiskComments = "sensitivity comments";
            application.CapacityRiskMitigated = true;
            application.CapacityRisks = new[] { "capacity risk 1", "capacity risk 2" };
            application.CapacityRiskComments = "capacity comments";
            application.RiskTransferMigigated = true;
            application.IncreasedOrTransferred = new[] { EMCR.DRR.Controllers.IncreasedOrTransferred.Increased };
            application.IncreasedOrTransferredComments = "transfer comments";

            //Budget - 10
            application.TotalProjectCost = 5000;
            application.YearOverYearFunding = new[] { new EMCR.DRR.Controllers.YearOverYearFunding { Amount = 4400, Year = "2024/2025" } };
            //application.TotalDrifFundingRequest = 5000;
            //application.RemainingAmount = 4300;
            application.DiscrepancyComment = "discrepancy comment";
            //application.CostEffective = false;
            application.IntendToSecureFunding = "intend to secure funding";
            application.CostEffectiveComments = "cost effective comments";
            application.PreviousResponse = EMCR.DRR.Controllers.YesNoOption.No;
            application.PreviousResponseCost = 1200;
            application.PreviousResponseComments = "previous response comments";
            application.CostConsiderationsApplied = true;
            application.CostConsiderations = new[] { "cost consideration 1", "cost consideration 2" };
            application.CostConsiderationsComments = "cost consideration comments";

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
