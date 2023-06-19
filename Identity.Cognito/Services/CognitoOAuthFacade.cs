using System.Text;
using Identity.Cognito.Models.Config;
using Identity.Shared.Builders;
using Identity.Shared.Interfaces;
using Identity.Shared.Models.Builders;
using Identity.Shared.Models.Clients;
using Identity.Shared.Models.OAuth;
using Microsoft.Extensions.Options;

namespace Identity.Cognito.Services;

public class CognitoOAuthFacade : IOAuthFacade
{
    private readonly IRestClient _httpClient;
    private readonly CognitoConfiguration _options;

    public CognitoOAuthFacade(IOptions<CognitoConfiguration> options, IRestClient httpClient)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<OAuthResponse> GetAccessTokenAsync(string code, string redirectUri)
    {
        var request = GetBaseRequestBuilder("authorization_code")
                .WithPathParameter("code", Uri.EscapeDataString(code))
                .WithPathParameter("redirect_uri", _options.RedirectUri)
                .Build();

        return await InnerExecuteAsync(request);
    }

    public async Task<OAuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var request = GetBaseRequestBuilder("refresh_token")
            .WithPathParameter("refresh_token", refreshToken)
            .Build();

        return await InnerExecuteAsync(request);
    }

    public async Task<OAuthResponse> GetClientCredentialsTokenAsync()
    {
        var request = GetBaseRequestBuilder("client_credentials")
            .Build();

        return await InnerExecuteAsync(request);
    }

    private HttpRequestBuilder GetBaseRequestBuilder(string grantType)
        => HttpRequestBuilder.Post("oauth2/token")
            .WithPathParameter("client_id", _options.ClientId)
            .WithPathParameter("client_secret", _options.ClientSecret)
            .WithPathParameter("grant_type", grantType)
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .WithHeader("Authorization", GetBasicAuthHeader());

    private async Task<OAuthResponse> InnerExecuteAsync(HttpRequestData data)
        => await _httpClient.ExecuteAsync<OAuthResponse>(data);
    
    private string GetBasicAuthHeader()
        => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"))}";
}