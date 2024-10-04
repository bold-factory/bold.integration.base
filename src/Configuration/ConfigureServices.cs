using Bold.Integration.Base.Authentication;
using Bold.Integration.Base.Middlewares;
using Bold.Integration.Base.Persistence;
using Bold.Integration.Base.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bold.Integration.Base.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.LoadConfiguration();
        services.AddLogging(builder =>
        {
            var options = builder.Services.GetOptions<SendGridSettings>();
            builder.AddProvider(new EmailLoggerProvider(new EmailService(options)));
        });

        services.ConfigureCors()
                .AddObservability()
                .AddApiClients();

        services.AddTransient<BoldAuthHandler>()
                .AddScoped<SyncStateService>()
                .AddScoped<ErrorLoggerService>()
                .AddScoped<SkusService>()
                .AddSingleton<AzureAdTokenService>();

        services.AddDbContext<IntegrationDbContext>((sp, c) =>
        {
            var databaseOptions = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            c.UseSqlServer(databaseOptions.ConnectionString);
        });

        services.AddHealthChecks();

        services.AddHostedService<ErpToBoldService>();
        return services;
    }
    public static TOptions GetOptions<TOptions>(this IServiceCollection services) where TOptions : class
    {
        return services.BuildServiceProvider().GetRequiredService<IOptions<TOptions>>().Value;
    }
}