using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Ok();
        }
    }
}
