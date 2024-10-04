using System.Net.Http.Headers;

namespace Bold.Integration.Base.Authentication;

public class BoldAuthHandler(AzureAdTokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await tokenService.GetAccessTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: accessToken);
        return await base.SendAsync(request: request, cancellationToken: cancellationToken);
    }
}