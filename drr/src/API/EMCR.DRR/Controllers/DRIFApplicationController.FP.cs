using EMCR.DRR.Managers.Intake;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.Controllers
{
    public partial class DRIFApplicationController
    {
        [HttpGet("FP/{id}")]
        public async Task<ActionResult<DraftFpApplication>> GetFP(string id)
        {
            var application = (await intakeManager.Handle(new DrrApplicationsQuery { Id = id, BusinessId = GetCurrentBusinessId() })).Items.FirstOrDefault();
            if (application == null) return new NotFoundObjectResult(new ProblemDetails { Type = "NotFoundException", Title = "Not Found", Detail = "" });
            return Ok(mapper.Map<DraftFpApplication>(application));
        }

        [HttpPost("FP/EOI/{eoiId}")]
        public async Task<ActionResult<ApplicationResult>> CreateFPFromEOI(string eoiId)
        {
            var id = await intakeManager.Handle(new CreateFpFromEoiCommand { EoiId = eoiId, UserInfo = GetCurrentUser() });
            return Ok(new ApplicationResult { Id = id });
        }

        [HttpPost("FP/{id}")]
        public async Task<ActionResult<ApplicationResult>> UpdateFPApplication([FromBody] DraftFpApplication application, string id)
        {
            //application.Id = id;
            //application.Status = SubmissionPortalStatus.Draft;
            //application.AdditionalContacts = MapAdditionalContacts(application);

            //var drr_id = await intakeManager.Handle(new DrifFpApplicationCommand { application = mapper.Map<FpApplication>(application), UserInfo = GetCurrentUser() });
            //return Ok(new ApplicationResult { Id = drr_id });
            await Task.CompletedTask;
            return Ok(new ApplicationResult { Id = "DRIF-FP-1000" });
        }

        [HttpPost("FP/{id}/submit")]
        public async Task<ActionResult<ApplicationResult>> SubmitFPApplication([FromBody] FpApplication application, string id)
        {
            //application.Id = id;
            //application.Status = SubmissionPortalStatus.UnderReview;
            //application.AdditionalContacts = MapAdditionalContacts(application);

            //var drr_id = await intakeManager.Handle(new DrifFpApplicationCommand { application = application, UserInfo = GetCurrentUser() });
            //return Ok(new ApplicationResult { Id = drr_id });
            await Task.CompletedTask;
            return Ok(new ApplicationResult { Id = "DRIF-FP-1000" });
        }
    }
}
