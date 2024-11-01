using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using EMCR.DRR.API.Services;
using EMCR.DRR.API.Services.S3;
using EMCR.DRR.Controllers;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;
using EMCR.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;
using Xrm.Tools.WebAPI;
using Xrm.Tools.WebAPI.Requests;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

#pragma warning disable CS8604 // Possible null reference argument.
builder.Host.UseSerilog((ctx, services, config) => Logging.ConfigureSerilog(ctx, services, config, configuration.GetValue("APP_NAME", string.Empty)));
#pragma warning restore CS8604 // Possible null reference argument.
services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
services.AddRouting(o => o.LowercaseUrls = true);
services.AddEndpointsApiExplorer();
services.AddIntakeManager();
services.AddRepositories();
services.AddS3Storage(configuration);
services.AddTransient<IUserService, UserService>();
services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddAutoMapper(typeof(ApplicationMapperProfile));
services.AddAutoMapper(typeof(IntakeMapperProfile));
services.AddSingleton(sp =>
{
    var dynamicsApiEndpoint = configuration.GetValue<string>("Dynamics:DynamicsApiEndpoint");
    var tokenProvider = sp.GetRequiredService<ISecurityTokenProvider>();
    return new CRMWebAPI(new CRMWebAPIConfig
    {
        APIUrl = dynamicsApiEndpoint + "/api/data/v9.0/",
        GetAccessToken = async (s) => await tokenProvider.AcquireToken()
    });
});
services.AddCors(opts => opts.AddDefaultPolicy(policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();

    //policy.WithOrigins("https://dev-drr-emcr.silver.devops.bcgov");


    //// try to get array of origins from section array
    //var corsOrigins = configuration.GetSection("cors:origins").GetChildren().Select(c => c.Value ?? string.Empty).ToArray();
    //// try to get array of origins from value
    //if (!corsOrigins.Any()) corsOrigins = configuration.GetValue("cors:origins", string.Empty)?.Split(',') ?? Array.Empty<string>();
    //corsOrigins = corsOrigins.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
    //if (corsOrigins.Any())
    //{
    //    policy.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(corsOrigins);
    //}
}));
services.AddCache(string.Empty)
    .AddDRRDynamics(builder.Configuration);

var defaultScheme = "Bearer_OR_SSO";

#pragma warning disable CS8601 // Possible null reference assignment.
services.AddAuthentication(options =>
{
    options.DefaultScheme = defaultScheme;
    options.DefaultChallengeScheme = defaultScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.MetadataAddress = configuration.GetValue<string>("jwt:metadataAddress");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        RequireSignedTokens = true,
        RequireAudience = true,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(60),
        NameClaimType = ClaimTypes.Upn,
        RoleClaimType = ClaimTypes.Role,
        ValidateActor = true,
        ValidateIssuerSigningKey = true,
    };

    configuration.GetSection("jwt").Bind(options);

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async c =>
        {
            var userService = c.HttpContext.RequestServices.GetRequiredService<IUserService>();
            Claim[] pathClaim =
            [
                new Claim("path", c.HttpContext.Request.Path.Value),
            ];
            c.Principal = new ClaimsPrincipal(new ClaimsIdentity(c.Principal.Identity, c.Principal.Claims.Concat(pathClaim)));

            c.Principal = await userService.GetPrincipal(c.Principal);
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
    };
    options.Validate();
})
.AddJwtBearer("SSO", options =>
{
    options.MetadataAddress = configuration.GetValue<string>("SSO:jwt:metadataAddress");
#pragma warning restore CS8601 // Possible null reference assignment.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        RequireSignedTokens = true,
        RequireAudience = true,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(60),
        NameClaimType = ClaimTypes.Upn,
        RoleClaimType = ClaimTypes.Role,
        ValidateActor = true,
        ValidateIssuerSigningKey = true,
    };

    configuration.GetSection("SSO:jwt").Bind(options);
    options.Validate();
})
.AddPolicyScheme(defaultScheme, defaultScheme, options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string? authorization = context.Request.Headers[HeaderNames.Authorization];

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            var token = authorization["Bearer ".Length..].Trim();
            var jwtHandler = new JwtSecurityTokenHandler();

            if (jwtHandler.CanReadToken(token))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);
                var identityProviderClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "aud");
                if (identityProviderClaim != null && identityProviderClaim.Value.Equals(configuration.GetValue<string>("SSO:jwt:audience"), StringComparison.InvariantCultureIgnoreCase))
                {
                    return "SSO";
                }
                else
                    return JwtBearerDefaults.AuthenticationScheme;
            }
            return JwtBearerDefaults.AuthenticationScheme;
        }
        return JwtBearerDefaults.AuthenticationScheme;
    };
    options.Validate();
});

services.AddAuthorization(options =>
{
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireClaim("user_info");
    });
    var ssoPolicyBuilder = new AuthorizationPolicyBuilder("SSO");
    options.AddPolicy("OnlySSO", ssoPolicyBuilder.RequireAuthenticatedUser().Build());

    //options.DefaultPolicy = options.GetPolicy(JwtBearerDefaults.AuthenticationScheme) ?? null!;
});

services.Configure<OpenApiDocumentMiddlewareSettings>(options =>
{
    options.Path = "/api/openapi/{documentName}/openapi.json";
    options.DocumentName = "DRR API";
    options.PostProcess = (document, req) =>
    {
        document.Info.Title = "DRR API";
    };
});

services.Configure<SwaggerUi3Settings>(options =>
{
    options.Path = "/api/openapi";
    options.DocumentTitle = "DRR API Documentation";
    options.DocumentPath = "/api/openapi/{documentName}/openapi.json";
});

services.AddOpenApiDocument(document =>
{
    document.AddSecurity("bearer token", Array.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "paste token here",
        In = OpenApiSecurityApiKeyLocation.Header
    });

    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer token"));
    document.GenerateAbstractProperties = true;
});

services.AddHealthChecks()
    .AddCheck($"ready hc", () => HealthCheckResult.Healthy("ready"), new[] { "ready" })
    .AddCheck($"live hc", () => HealthCheckResult.Healthy("alive"), new[] { "alive" });

var app = builder.Build();

app.MapHealthChecks("/hc/ready", new HealthCheckOptions() { Predicate = check => check.Tags.Contains("ready") });
app.MapHealthChecks("/hc/live", new HealthCheckOptions() { Predicate = check => check.Tags.Contains("live") });

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
