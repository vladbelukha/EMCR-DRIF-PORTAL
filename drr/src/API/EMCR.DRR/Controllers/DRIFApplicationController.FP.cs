using EMCR.DRR.API.Model;
using EMCR.DRR.Managers.Intake;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.Controllers
{
    public partial class DRIFApplicationController
    {
        [HttpGet("fp/{id}")]
        public async Task<ActionResult<DraftFpApplication>> GetFP(string id)
        {
            try
            {
                var application = (await intakeManager.Handle(new DrrApplicationsQuery { Id = id, BusinessId = GetCurrentBusinessId() })).Items.FirstOrDefault();
                if (application == null) return new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = "" });
                return Ok(mapper.Map<DraftFpApplication>(application));
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpPost("fp")]
        public async Task<ActionResult<ApplicationResult>> CreateFPFromEOI([FromQuery] string eoiId, [FromBody] ScreenerQuestions screenerQuestions)
        {
            try
            {
                if (string.IsNullOrEmpty(eoiId)) throw new ArgumentNullException(nameof(eoiId));
                var id = await intakeManager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, ScreenerQuestions = mapper.Map<Managers.Intake.ScreenerQuestions>(screenerQuestions), UserInfo = GetCurrentUser() });
                return Ok(new ApplicationResult { Id = id });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpPost("fp/{id}")]
        public async Task<ActionResult<ApplicationResult>> UpdateFPApplication([FromBody] DraftFpApplication application, string id)
        {
            try
            {
                application.Id = id;
                application.Status = SubmissionPortalStatus.Draft;
                application.AdditionalContacts = MapAdditionalContacts(application);

                var drr_id = await intakeManager.Handle(new FpSaveApplicationCommand { application = mapper.Map<FpApplication>(application), UserInfo = GetCurrentUser() });
                return Ok(new ApplicationResult { Id = drr_id });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }

        [HttpPost("fp/{id}/submit")]
        public async Task<ActionResult<ApplicationResult>> SubmitFPApplication([FromBody] FpApplication application, string id)
        {
            try
            {
                application.Id = id;
                application.Status = SubmissionPortalStatus.UnderReview;
                application.AdditionalContacts = MapAdditionalContacts(application);

                var drr_id = await intakeManager.Handle(new FpSubmitApplicationCommand { application = application, UserInfo = GetCurrentUser() });
                return Ok(new ApplicationResult { Id = drr_id });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e, logger);
            }
        }
    }


    public class ScreenerQuestions
    {
        public bool? ProjectWorkplan { get; set; }
        public bool? ProjectSchedule { get; set; }
        public bool? CostEstimate { get; set; }
        public YesNoOption? SitePlan { get; set; }
        public bool? HaveAuthorityToDevelop { get; set; }
        public YesNoOption? FirstNationsAuthorizedByPartners { get; set; }
        public YesNoOption? LocalGovernmentAuthorizedByPartners { get; set; }
        public YesNoOption? FoundationWorkCompleted { get; set; }
        public bool? EngagedWithFirstNationsOccurred { get; set; }
        public bool? IncorporateFutureClimateConditions { get; set; }
        public bool? MeetsRegulatoryRequirements { get; set; }
        public bool? MeetsEligibilityRequirements { get; set; }
    }
}
