using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace SmsIntegration.Extensions
{
    public static class LoggingExtensions
    {

        public static void AddLogging(this WebApplicationBuilder builder)
        {
            var baseAddress = builder.Configuration["NewLoggingElasticSearchURL"];
            var templateVersion = Enum.TryParse(builder.Configuration["ElasticsearchTemplateVersion"],
                out AutoRegisterTemplateVersion parsedVersion)
                ? parsedVersion
                : AutoRegisterTemplateVersion.ESv7;
            var applicationName = "sms-integration";
            var applicationVersion = "V1";

            Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", applicationVersion)
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithProperty("HostName", Environment.MachineName)
            .Enrich.FromLogContext()
            .MinimumLevel.Warning()
            .MinimumLevel.Override("Sms", LogEventLevel.Information)
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(baseAddress))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = templateVersion,
                ModifyConnectionSettings = x =>
                {
                    if (builder.Configuration["ElasticsearchUser"] != null)
                    {
                        x.BasicAuthentication(
                            builder.Configuration["ElasticsearchUser"],
                            builder.Configuration["ElasticsearchPassword"]);
                    }

                    x.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
                    return x;
                },
                OverwriteTemplate = true,
                NumberOfReplicas = 0,
                NumberOfShards = 1,
                IndexFormat = "sms-integration-{0:yyyy.MM}"
            })
            .CreateLogger();
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        }
    }
}
