using System.Net.Mime;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace EMCR.DRR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class VersionController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VersionInformation>>> GetVersionInformation()
        {
            await Task.CompletedTask;
            var version = Environment.GetEnvironmentVariable("VERSION") ?? string.Empty;
            var name = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            return Ok(new[] { new VersionInformation { Version = version, Name = name } });
        }
    }

    public class VersionInformation
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
    }
}
