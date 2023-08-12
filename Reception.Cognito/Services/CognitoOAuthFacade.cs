using System.Text;
using System.Web;
using Reception.Cognito.Models.Config;
using Reception.Shared.Builders;
using Reception.Shared.Interfaces;
using Reception.Shared.Models.Builders;
using Reception.Shared.Models.Clients;
using Reception.Shared.Models.OAuth;
using Microsoft.Extensions.Options;

namespace Reception.Cognito.Services;

public class CognitoOAuthFacade : IOAuthFacade
{
    private readonly IRestClient _httpClient;
    private readonly CognitoConfiguration _options;

    public CognitoOAuthFacade(IOptions<CognitoConfiguration> options, IRestClient httpClient)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<OAuthResponse> GetAccessTokenAsync(string code)
    {
        var request = GetBaseRequestBuilder(
                "authorization_code", 
                _options.UserCredentialsFlow.ClientId,
                _options.UserCredentialsFlow.ClientSecret,
                _options.UserCredentialsFlow.Scopes
            )
            .WithQueryParameter("code", Uri.EscapeDataString(code))
            .WithQueryParameter("redirect_uri", _options.UserCredentialsFlow.RedirectUri)
            .Build();

        return await InnerExecuteAsync(request);
    }

    public async Task<OAuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var request = GetBaseRequestBuilder(
                "refresh_token", 
                _options.UserCredentialsFlow.ClientId,
                _options.UserCredentialsFlow.ClientSecret,
                _options.UserCredentialsFlow.Scopes
            )
            .WithQueryParameter("refresh_token", refreshToken)
            .Build();

        return await InnerExecuteAsync(request);
    }

    public async Task<OAuthResponse> GetClientCredentialsTokenAsync()
    {
        var request = GetBaseRequestBuilder(
                "client_credentials",
                _options.ClientCredentialsFlow.ClientId,
                _options.ClientCredentialsFlow.ClientSecret,
                _options.ClientCredentialsFlow.Scopes
            )
            .Build();

        return await InnerExecuteAsync(request);
    }

    public Uri GetLoginForm()
    {
        var builder = new UriBuilder(_options.BaseUrl + "login");
        var query = HttpUtility.ParseQueryString("");
        
        query.Add("response_type", "code");
        query.Add("client_id", _options.UserCredentialsFlow.ClientId);
        query.Add("scope", string.Join("+",_options.UserCredentialsFlow.Scopes));
        query.Add("redirect_uri", _options.UserCredentialsFlow.RedirectUri.ToString());
        
        builder.Query = query.ToString();

        return builder.Uri;
    }

    private HttpRequestBuilder GetBaseRequestBuilder(string grantType, string clientId, string clientSecret, string[] scopes)
        => HttpRequestBuilder.Post("oauth2/token")
            .WithQueryParameter("client_id", clientId)
            .WithQueryParameter("client_secret", clientSecret)
            .WithQueryParameter("grant_type", grantType)
            .WithQueryParameter("scope", string.Join(" ", scopes))
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .WithHeader("Authorization", GetBasicAuthHeader(clientId, clientSecret));

    private async Task<OAuthResponse> InnerExecuteAsync(HttpRequestData data)
        => await _httpClient.ExecuteAsync<OAuthResponse>(data);

    private string GetBasicAuthHeader(string clientId, string clientSecret)
        => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))}";
}