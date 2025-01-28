using System.Net;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;

namespace EMCR.DRR.API.Services
{
    public static class Logging
    {
        public const string LogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}";

        public static void ConfigureSerilog(HostBuilderContext hostBuilderContext, IServiceProvider services, LoggerConfiguration loggerConfiguration, string appName)
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostBuilderContext.Configuration)
                .ReadFrom.Services(services)
                .Enrich.WithMachineName()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("app", appName)
                .Enrich.WithEnvironmentName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithCorrelationId()
                .Enrich.WithCorrelationIdHeader()
                .Enrich.WithClientIp()
                .Enrich.WithSpan()
                //.MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.OData.Extensions", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: LogOutputTemplate)
                ;

            var splunkUrl = hostBuilderContext.Configuration.GetValue("SPLUNK_URL", string.Empty);
            var splunkToken = hostBuilderContext.Configuration.GetValue("SPLUNK_TOKEN", string.Empty);
            if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
            {
                Log.Warning($"Logs will NOT be forwarded to Splunk: check SPLUNK_TOKEN and SPLUNK_URL env vars");
            }
            else
            {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                loggerConfiguration
                    .WriteTo.EventCollector(
                        splunkHost: splunkUrl,
                        eventCollectorToken: splunkToken,
                        messageHandler: new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        },
                        renderTemplate: false);
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                Log.Information($"Logs will be forwarded to Splunk");
            }
        }

        public static IApplicationBuilder SetDefaultRequestLogging(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSerilogRequestLogging(opts =>
            {
                opts.IncludeQueryInRequestPath = true;
                opts.GetLevel = ExcludeHealthChecks;
            });

            return applicationBuilder;
        }

        private static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex)
        {
            if (ex != null || ctx.Response.StatusCode >= (int)HttpStatusCode.InternalServerError) return LogEventLevel.Error;
            return ctx.Request.Path.StartsWithSegments("/hc", StringComparison.InvariantCultureIgnoreCase)
                    ? LogEventLevel.Verbose
                    : LogEventLevel.Information;
        }
    }
}
