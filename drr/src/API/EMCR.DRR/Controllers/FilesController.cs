using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.Claims;
using AutoMapper;
using EMCR.DRR.API.Services;
using EMCR.DRR.Controllers;
using EMCR.DRR.Managers.Intake;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<AttachmentController> logger;
        private readonly IIntakeManager intakeManager;
        private readonly IMapper mapper;
        private readonly ErrorParser errorParser;

#pragma warning disable CS8603 // Possible null reference return.
        private string GetCurrentBusinessId() => User.FindFirstValue("bceid_business_guid");
        private string GetCurrentBusinessName() => User.FindFirstValue("bceid_business_name");
        private string GetCurrentUserId() => User.FindFirstValue("bceid_user_guid");
        private UserInfo GetCurrentUser()
        {
            return new UserInfo { BusinessId = GetCurrentBusinessId(), BusinessName = GetCurrentBusinessName(), UserId = GetCurrentUserId() };
        }
#pragma warning restore CS8603 // Possible null reference return.

        public FilesController(ILogger<AttachmentController> logger, IIntakeManager intakeManager, IMapper mapper)
        {
            this.logger = logger;
            this.intakeManager = intakeManager;
            this.mapper = mapper;
            this.errorParser = new ErrorParser();
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<ApplicationResult>> UploadFile(
            [FromForm] UploadFileRequest request,
        [FromRoute] Guid fileId,
        [FromHeader(Name = "file-classification")] string? classification,
        [FromHeader(Name = "file-tag")] string? tags,
        [FromHeader(Name = "file-folder")] string? folder,
        CancellationToken ct
            )
        {
            Console.WriteLine("UploadFile " + fileId);
            Console.WriteLine(request.File.FileName);
            var bytes = await GetBytes(request.File);
            Console.WriteLine(bytes.Length);
            await Task.CompletedTask;
            return Ok(new ApplicationResult { Id = "fileId" });
        }


        public static async Task<byte[]> GetBytes(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public class UploadFileRequest
    {
        [Required(ErrorMessage = "Please add a file")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IFormFile File { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
