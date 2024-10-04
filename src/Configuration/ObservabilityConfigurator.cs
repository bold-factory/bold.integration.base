using Azure.Monitor.OpenTelemetry.Exporter;
using Bold.Integration.Base.Observability;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Bold.Integration.Base.Configuration;

public static class ObservabilityConfigurator
{
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        var azure = services.GetOptions<AzureSettings>();
        var azureMonitorConnectionString = azure.Monitor?.ConnectionString;
        services
            .AddOpenTelemetry()
            .ConfigureResource(builder => builder.AddService("integration-erp"))
            .WithTracing(builder =>
            {
                builder.AddSource(Diagnostics.SourceName)
                       .AddHttpClientInstrumentation()
                       .AddAspNetCoreInstrumentation()
                       .AddEntityFrameworkCoreInstrumentation();
                if (!string.IsNullOrEmpty(azureMonitorConnectionString))
                {
                    builder.AddAzureMonitorTraceExporter(config =>
                    {
                        config.ConnectionString = azureMonitorConnectionString;
                    });
                }
                builder.AddOtlpExporter();
            })
            .WithMetrics(builder =>
            {
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();
                if (!string.IsNullOrEmpty(azureMonitorConnectionString))
                {
                    builder.AddAzureMonitorMetricExporter(config =>
                    {
                        config.ConnectionString = azureMonitorConnectionString;
                    });
                }
                builder.AddOtlpExporter();
            });
        services.AddLogging(lg => lg.AddOpenTelemetry(builder =>
        {
            if (!string.IsNullOrEmpty(azureMonitorConnectionString))
            {
                builder.AddAzureMonitorLogExporter(config =>
                {
                    config.ConnectionString = azureMonitorConnectionString;
                });
            }
            builder.AddOtlpExporter();
        }));
        return services;
    }
}