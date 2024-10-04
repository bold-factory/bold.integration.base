using Bold.Integration.Base.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Bold.Integration.Base.Authentication;

public class AzureAdTokenService
{
    private readonly IConfidentialClientApplication _clientApp;
    private readonly AzureSettings _configuration;
    private AuthenticationResult? _cachedToken;
    public AzureAdTokenService(IOptions<AzureSettings> configuration)
    {
        _configuration = configuration.Value;
        var authority = "https://login.microsoftonline.com/"+_configuration.TenantId;
        _clientApp = ConfidentialClientApplicationBuilder.Create(_configuration.AppId)
                                                         .WithClientSecret(_configuration.AppSecret)
                                                         .WithAuthority(new Uri(authority))
                                                         .Build();
    }
    public async Task<string> GetAccessTokenAsync()
    {
        // If there's a cached token and it's still valid, return it
        if (_cachedToken != null && _cachedToken.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            return _cachedToken.AccessToken;
        }

        // Otherwise, acquire a new token
        _cachedToken = await _clientApp.AcquireTokenForClient([_configuration.Scope]).ExecuteAsync();

        return _cachedToken.AccessToken;
    }
}