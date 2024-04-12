using EMCR.DRR.Dynamics;
using EMCR.Utilities;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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


var app = builder.Build();

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
