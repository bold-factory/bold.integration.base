namespace Bold.Integration.Base.Configuration;

public static class CorsConfigurator
{
    public const string ProductionOrigins = "_productionOrigins";
    public const string DevelopmentOrigins = "_developmentOrigins";
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: ProductionOrigins,
                              policy =>
                              {
                                  policy.WithOrigins("https://api.bold-factory.com")
                                        .AllowAnyHeader()
                                        .AllowCredentials()
                                        .AllowAnyMethod();
                              });
            options.AddPolicy(name: DevelopmentOrigins,
                              policy =>
                              {
                                  policy.WithOrigins("https://localhost:3001", "http://localhost:3000")
                                        .AllowAnyHeader()
                                        .AllowCredentials()
                                        .AllowAnyMethod();
                              });
        });
        return services;
    }
}