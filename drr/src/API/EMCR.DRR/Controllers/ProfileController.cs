using System.Net.Mime;
using System.Security.Claims;
using BCeIDService;
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
        private string GetCurrentBusinessName() => User.FindFirstValue("bceid_business_name");
        private string GetCurrentBusinessId() => User.FindFirstValue("bceid_business_guid");
        private string GetCurrentUserId() => User.FindFirstValue("bceid_user_guid");
#pragma warning restore CS8603 // Possible null reference return.

        private readonly IConfiguration configuration;
        private readonly IIntakeManager intakeManager;

        public ProfileController(IConfiguration configuration, IIntakeManager intakeManager)
        {
            this.configuration = configuration;
            this.intakeManager = intakeManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDetails>> ProfileDetails()
        {
            BCeIDServiceSoapClient client = new BCeIDServiceSoapClient(
                configuration.GetValue<string>("BCeID:SOAPURL"),
                TimeSpan.FromSeconds(60),
                configuration.GetValue<string>("BCeID:serviceAccountName"),
                configuration.GetValue<string>("BCeID:serviceAcountPassword")
                );

            var accountDetails = await client.getAccountDetailAsync(new AccountDetailRequest
            {
                onlineServiceId = configuration.GetValue<string>("BCeID:serviceAccountId"),
                requesterAccountTypeCode = BCeIDAccountTypeCode.Business,
                requesterUserGuid = GetCurrentUserId(),
                userGuid = GetCurrentUserId(),
                accountTypeCode = BCeIDAccountTypeCode.Business,
            });

            if (accountDetails == null) return NotFound("BCeID account details not found");
            var profile = new ProfileDetails
            {
                BusinessName = GetCurrentBusinessName(),
                FirstName = accountDetails.account.individualIdentity.name.firstname.value,
                LastName = accountDetails.account.individualIdentity.name.surname.value,
                Title = accountDetails.account.internalIdentity.title.value,
                Department = accountDetails.account.internalIdentity.department.value,
                Phone = accountDetails.account.contact.telephone.value,
                Email = accountDetails.account.contact.email.value,
            };

            return Ok(await Task.FromResult(profile));
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
