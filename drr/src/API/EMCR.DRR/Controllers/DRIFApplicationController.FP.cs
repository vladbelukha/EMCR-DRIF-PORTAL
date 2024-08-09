using EMCR.DRR.API.Services;
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
                return errorParser.Parse(e);
            }
        }

        [HttpPost("fp")]
        public async Task<ActionResult<ApplicationResult>> CreateFPFromEOI(string eoiId)
        {
            try
            {
                if (string.IsNullOrEmpty(eoiId)) throw new ArgumentNullException(nameof(eoiId));
                var id = await intakeManager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetCurrentUser() });
                return Ok(new ApplicationResult { Id = id });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e);
            }
        }

        [HttpPost("fp/{id}")]
        public async Task<ActionResult<ApplicationResult>> UpdateFPApplication([FromBody] DraftFpApplication application, string id)
        {
            try
            {
                //application.Id = id;
                //application.Status = SubmissionPortalStatus.Draft;
                //application.AdditionalContacts = MapAdditionalContacts(application);

                //var drr_id = await intakeManager.Handle(new DrifFpApplicationCommand { application = mapper.Map<FpApplication>(application), UserInfo = GetCurrentUser() });
                //return Ok(new ApplicationResult { Id = drr_id });
                await Task.CompletedTask;
                return Ok(new ApplicationResult { Id = "DRIF-FP-1000" });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e);
            }
        }

        [HttpPost("fp/{id}/submit")]
        public async Task<ActionResult<ApplicationResult>> SubmitFPApplication([FromBody] FpApplication application, string id)
        {
            try
            {
                //application.Id = id;
                //application.Status = SubmissionPortalStatus.UnderReview;
                //application.AdditionalContacts = MapAdditionalContacts(application);

                //var drr_id = await intakeManager.Handle(new DrifFpApplicationCommand { application = application, UserInfo = GetCurrentUser() });
                //return Ok(new ApplicationResult { Id = drr_id });
                await Task.CompletedTask;
                return Ok(new ApplicationResult { Id = "DRIF-FP-1000" });
            }
            catch (Exception e)
            {
                return errorParser.Parse(e);
            }
        }
    }
}
