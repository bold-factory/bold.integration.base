using Bold.Integration.Base.Authentication;
using Bold.Integration.Base.Clients;
using Polly;
using Polly.Retry;

namespace Bold.Integration.Base.Configuration;

public static class ClientsConfigurator
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        var apiSettings = services.GetOptions<ApiSettings>();

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

        services.AddHttpClient(name: "bold", client =>
                {
                    var baseUrl = apiSettings.Bold.BaseUrl;
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromSeconds(20);
                }).AddHttpMessageHandler<BoldAuthHandler>()
                .AddPolicyHandler((sp, _) => RetryPolicy(sp));

        services.AddScoped<BoldClient>(x =>
        {
            var baseUrl = apiSettings.Bold.BaseUrl;
            var clientFactory = x.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("bold");
            var boldClient = new BoldClient(baseUrl: baseUrl, httpClient: client)
            {
                ReadResponseAsString = false
            };
            return boldClient;
        });

        return services;
    }
}