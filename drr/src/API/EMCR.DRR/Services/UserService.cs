using System.Security.Claims;
using EMCR.Utilities;
using Microsoft.Extensions.Caching.Distributed;

namespace EMBC.DRR.API.Services;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE0022 // Use block body for method
#pragma warning disable CS8603 // Possible null reference return.
public interface IUserService
{
    Task<ClaimsPrincipal> GetPrincipal(ClaimsPrincipal sourcePrincipal = null);
    Task<TeamMember> GetTeamMember(string userName = null);
}

public class UserService(IDistributedCache cache, IHttpContextAccessor httpContext) : IUserService
{
    private ClaimsPrincipal? currentPrincipal => httpContext.HttpContext?.User;
    private static string GetCurrentUserName(ClaimsPrincipal principal) => principal.FindFirstValue("bceid_business_name");

    public async Task<ClaimsPrincipal> GetPrincipal(ClaimsPrincipal? sourcePrincipal = null)
    {
        if (sourcePrincipal == null) sourcePrincipal = currentPrincipal;
        var userName = GetCurrentUserName(sourcePrincipal);


        var cacheKey = $"user:{userName}";
        var teamMember = await cache.GetOrSet(cacheKey, async () => await GetTeamMember(userName), TimeSpan.FromMinutes(10));
        if (teamMember == null) return sourcePrincipal;

        Claim[] drrClaims =
        [
            new Claim("user_id", teamMember.Id),
            new Claim("user_role", teamMember.Role),
            new Claim("user_team", teamMember.TeamId)
        ];
        return new ClaimsPrincipal(new ClaimsIdentity(sourcePrincipal.Identity, sourcePrincipal.Claims.Concat(drrClaims)));
    }

    public async Task<TeamMember> GetTeamMember(string? userName = null)
    {
        if (string.IsNullOrEmpty(userName)) userName = GetCurrentUserName(currentPrincipal);
        await Task.CompletedTask;
        return new TeamMember { Id = Guid.NewGuid().ToString(), Role = "role", TeamId = Guid.NewGuid().ToString() };
        //TODO - add httpclient to query BCeID API to get additional user info
        //return (await messagingClient.Send(new TeamMembersQuery { UserName = userName, IncludeActiveUsersOnly = true })).TeamMembers.SingleOrDefault();
    }
}
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore IDE0022 // Use block body for method
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

public class TeamMember
{
    public required string Id { get; set; }
    public string? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? ExternalUserId { get; set; }
    public string? Role { get; set; }
    public string? Label { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
