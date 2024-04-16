using System.Text.Json.Serialization;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Resources.Applications;
using EMCR.Utilities;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddRouting(o => o.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
builder.Services.AddAutoMapper(typeof(ApplicationMapperProfile));
builder.Services.AddCache(string.Empty)
    .AddDRRDynamics(builder.Configuration);

builder.Services.Configure<OpenApiDocumentMiddlewareSettings>(options =>
{
    options.Path = "/api/openapi/{documentName}/openapi.json";
    options.DocumentName = "DRR API";
    options.PostProcess = (document, req) =>
    {
        document.Info.Title = "DRR API";
    };
});

builder.Services.Configure<SwaggerUi3Settings>(options =>
{
    options.Path = "/api/openapi";
    options.DocumentTitle = "DRR API Documentation";
    options.DocumentPath = "/api/openapi/{documentName}/openapi.json";
});

builder.Services.AddOpenApiDocument(document =>
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

builder.Services.AddHealthChecks()
    .AddCheck($"ready hc", () => HealthCheckResult.Healthy("ready"), new[] { "ready" })
    .AddCheck($"live hc", () => HealthCheckResult.Healthy("alive"), new[] { "alive" });

var app = builder.Build();

app.MapHealthChecks("/hc/ready", new HealthCheckOptions() { Predicate = check => check.Tags.Contains("ready") });
app.MapHealthChecks("/hc/live", new HealthCheckOptions() { Predicate = check => check.Tags.Contains("live") });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
