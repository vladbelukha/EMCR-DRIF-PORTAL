using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using EMCR.DRR.API.Services;
using EMCR.DRR.API.Services.S3;
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
        private readonly IS3Provider s3Provider;
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

        public FilesController(ILogger<AttachmentController> logger, IS3Provider s3Provider, IMapper mapper)
        {
            this.logger = logger;
            this.s3Provider = s3Provider;
            this.mapper = mapper;
            this.errorParser = new ErrorParser();
        }

#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        [HttpGet("{id}")]
        public async Task<FileStreamResult> DownloadFile(
            [FromRoute] string id,
            [FromHeader(Name = "file-folder")] string? folder
            )
        {
            Console.WriteLine("CRM - DownloadFile");
            Console.WriteLine(id);
            Console.WriteLine(folder);

            var res = (FileQueryResult)(await s3Provider.HandleQuery(new FileQuery { Key = id, Folder = folder }));
            var content = new MemoryStream(res.File.Content);
            var contentType = res.File.ContentType ?? "application/octet-stream";

            if (res.FileTag != null)
            {
                HttpContext.Response.Headers.Add(DrrHeaderNames.HEADER_FILE_CLASSIFICATION,
                    res.FileTag.Tags.SingleOrDefault(t => t.Key == DrrHeaderNames.HEADER_FILE_CLASSIFICATION)?.Value);

                string tagStr = GetStrFromTags(res.FileTag.Tags);
                if (!string.IsNullOrWhiteSpace(tagStr))
                    HttpContext.Response.Headers.Add(DrrHeaderNames.HEADER_FILE_TAG, tagStr);
            }

            if (!string.IsNullOrWhiteSpace(folder))
                HttpContext.Response.Headers.Add(DrrHeaderNames.HEADER_FILE_FOLDER, folder);

            return new FileStreamResult(content, contentType);
        }
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer


        [HttpPost("{id}")]
        public async Task<ActionResult<ApplicationResult>> UploadFile(
            [FromForm] UploadFileRequest request,
            [FromRoute] string id,
            [FromHeader(Name = "file-classification")] string? classification,
            [FromHeader(Name = "file-tag")] string? tags,
            [FromHeader(Name = "file-folder")] string? folder
            )
        {
            var bytes = await GetBytes(request.File);
            Console.WriteLine(request.File.FileName);
            Console.WriteLine(classification);
            Console.WriteLine(tags);
            Console.WriteLine(folder);
            var file = new S3File { FileName = request.File.FileName, Content = bytes, ContentType = request.File.ContentType };
            await s3Provider.HandleCommand(new UploadFileCommand { Folder = folder, Key = id, File = file });
            return Ok(new ApplicationResult { Id = id });
        }


        public static async Task<byte[]> GetBytes(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static string GetStrFromTags(IEnumerable<Tag> tags)
        {
            List<string> tagStrlist = new();
            foreach (Tag t in tags)
            {
                if (t.Key != DrrHeaderNames.HEADER_FILE_CLASSIFICATION)
                    tagStrlist.Add($"{t.Key}={t.Value}");
            }
            return string.Join(",", tagStrlist);
        }
    }

    public class UploadFileRequest
    {
        [Required(ErrorMessage = "Please add a file")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IFormFile File { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public static class DrrHeaderNames
    {
        public static readonly string HEADER_FILE_CLASSIFICATION = "file-classification";
        public static readonly string HEADER_FILE_TAG = "file-tag";
        public static readonly string HEADER_FILE_FOLDER = "file-folder";
    }
}
