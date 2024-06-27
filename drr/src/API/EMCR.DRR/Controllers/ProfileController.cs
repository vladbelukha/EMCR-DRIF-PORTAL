using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using EMBC.DRR.API.Services;
using EMCR.DRR.Managers.Intake;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize]
    public class ProfileController : Controller
    {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
        private AccountDetails GetCurrentUserInfo() => JsonSerializer.Deserialize<AccountDetails>(User.FindFirstValue("user_info"));
        private string GetCurrentBusinessName() => User.FindFirstValue("bceid_business_name");
        private string GetCurrentBusinessId() => User.FindFirstValue("bceid_business_guid");
        private string GetCurrentUserId() => User.FindFirstValue("bceid_user_guid");
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8603 // Possible null reference return.

        private readonly IConfiguration configuration;
        private readonly IIntakeManager intakeManager;
        private readonly IMapper mapper;

        public ProfileController(IConfiguration configuration, IIntakeManager intakeManager, IMapper mapper)
        {
            this.configuration = configuration;
            this.intakeManager = intakeManager;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDetails>> ProfileDetails()
        {
            var userInfo = GetCurrentUserInfo();
            return Ok(await Task.FromResult(mapper.Map<ProfileDetails>(userInfo)));
        }

        [HttpGet("exists")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> GetProfileExists()
        {
            var id = await intakeManager.Handle(new CheckProfileExists { BusinessId = GetCurrentBusinessId(), Name = GetCurrentBusinessName() });
            return Ok(!string.IsNullOrEmpty(id));
        }
    }

    public class ProfileDetails
    {
        public required string BusinessName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Title { get; set; }
        public string? Department { get; set; }
        public string? Phone { get; set; }
        public required string Email { get; set; }
    }
}
