namespace Bold.Integration.Base.Configuration;

public static class ConfigurationLoader
{
    public static IServiceCollection LoadConfiguration(this IServiceCollection services)
    {
        services.AddOptions<AzureSettings>().BindConfiguration("Azure").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<DatabaseSettings>().BindConfiguration("Database").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<ApiSettings>().BindConfiguration("ApiClients").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<SendGridSettings>().BindConfiguration("SendGrid").ValidateDataAnnotations().ValidateOnStart();
        return services;
    }
}