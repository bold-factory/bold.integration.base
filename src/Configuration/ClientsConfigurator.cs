using Bold.Integration.Base.Authentication;
using Bold.Integration.Base.Clients;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Bold.Integration.Base.Configuration;

public static class ClientsConfigurator
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        AsyncRetryPolicy<HttpResponseMessage> RetryPolicy(IServiceProvider sp)
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            return Policy.Handle<HttpRequestException>()
                         .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
                         .WaitAndRetryAsync(retryCount: 3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(x: 2, y: retryAttempt)),
                                            (r, timespan, retryCount, _) =>
                                            {
                                                logger.LogWarning(message:
                                                                  "Error: {error}, Response: {response}, Retrying for {retryCount} time, waiting {seconds}s.",
                                                                  r.Exception, r.Result, retryCount, timespan.TotalSeconds);
                                            });
        }

        services.AddHttpClient();

        services.AddHttpClient(name: nameof(BoldClient), (sp, client) =>
                {
                    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                    client.BaseAddress = new Uri(apiSettings.Bold.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(20);
                })
                .AddHttpMessageHandler<BoldAuthHandler>()
                .AddPolicyHandler((sp, _) => RetryPolicy(sp));


        services.AddScoped(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient(nameof(BoldClient)); // Create named HttpClient for BoldClient
            var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

            return new BoldClient(baseUrl: apiSettings.Bold.BaseUrl, httpClient: httpClient)
            {
                ReadResponseAsString = false
            };
        });
        
        services.AddScoped<ErpClient>();

        return services;
    }
}