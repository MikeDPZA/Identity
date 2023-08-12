using System.Web;
using Reception.Cognito.Models.Config;
using Reception.Shared.Builders;
using Reception.Shared.Interfaces;
using Reception.Shared.Models.Builders;
using Reception.Shared.Models.OAuth;
using Microsoft.Extensions.Options;
using Reception.Shared.Clients;
using Reception.Shared.Constants;
using Reception.Shared.Utilities;

namespace Reception.Cognito.Services;

public class CognitoOAuthTokenFacade : IOAuthTokenFacade
{
    private readonly IRestClient _httpClient;
    private readonly CognitoConfiguration _options;

    public CognitoOAuthTokenFacade(
        IOptions<CognitoConfiguration> options, 
        IRestClient httpClient)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<OAuthResponse> GetAccessTokenAsync(string code)
    {
        var request = GetBaseRequestBuilder(
                OAuthConstants.GrantTypeAuthCode, 
                _options.UserCredentialsFlow.ClientId,
                _options.UserCredentialsFlow.ClientSecret,
                _options.UserCredentialsFlow.Scopes
            )
            .WithQueryParameter(OAuthConstants.ResponseTypeCode, Uri.EscapeDataString(code))
            .WithQueryParameter(OAuthConstants.RedirectUri, _options.UserCredentialsFlow.RedirectUri)
            .Build();

        return await InnerExecuteAsync(request);
    }

    public async Task<OAuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var request = GetBaseRequestBuilder(
                OAuthConstants.GrantTypeRefreshToken, 
                _options.UserCredentialsFlow.ClientId,
                _options.UserCredentialsFlow.ClientSecret,
                _options.UserCredentialsFlow.Scopes
            )
            .WithQueryParameter(OAuthConstants.GrantTypeRefreshToken, refreshToken)
            .Build();

        return await InnerExecuteAsync(request);
    }

    public async Task<OAuthResponse> GetClientCredentialsTokenAsync()
    {
        var request = GetBaseRequestBuilder(
                OAuthConstants.GrantTypeClientCredentials,
                _options.ClientCredentialsFlow.ClientId,
                _options.ClientCredentialsFlow.ClientSecret,
                _options.ClientCredentialsFlow.Scopes
            )
            .Build();

        return await InnerExecuteAsync(request);
    }

    public Uri GetLoginUri(string? state)
    {
        var builder = new UriBuilder(_options.BaseUrl + "login");
        var query = HttpUtility.ParseQueryString("");
        
        query.Add(OAuthConstants.ResponseType, OAuthConstants.ResponseTypeCode);
        query.Add(OAuthConstants.ClientId, _options.UserCredentialsFlow.ClientId);
        query.Add(OAuthConstants.Scope, string.Join("+",_options.UserCredentialsFlow.Scopes));
        query.Add(OAuthConstants.RedirectUri, _options.UserCredentialsFlow.RedirectUri.ToString());
        if (!string.IsNullOrEmpty(state))
        {
            query.Add(OAuthConstants.State, Encoder.Encode64(state));
        }
        
        builder.Query = query.ToString();

        return builder.Uri;
    }

    public Uri GetLogoutUri(string? state)
    {
        var builder = new UriBuilder(_options.BaseUrl + "logout");
        var query = HttpUtility.ParseQueryString("");
        query.Add(OAuthConstants.ClientId, _options.UserCredentialsFlow.ClientId);
        query.Add(OAuthConstants.RedirectUri, _options.UserCredentialsFlow.RedirectUri.ToString());
        query.Add(OAuthConstants.ResponseType, OAuthConstants.ResponseTypeCode);
        if (!string.IsNullOrEmpty(state))
        {
            query.Add(OAuthConstants.State, Encoder.Encode64(state));
        }
        builder.Query = query.ToString();

        return builder.Uri;
    }

    private HttpRequestBuilder GetBaseRequestBuilder(string grantType, string clientId, string clientSecret, string[] scopes)
        => HttpRequestBuilder.Post(OAuthConstants.OAuth, OAuthConstants.Token)
            .WithQueryParameter(OAuthConstants.ClientId, clientId)
            .WithQueryParameter(OAuthConstants.ClientSecret, clientSecret)
            .WithQueryParameter(OAuthConstants.GrantType, grantType)
            .WithQueryParameter(OAuthConstants.Scope, string.Join(" ", scopes))
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .WithHeader("Authorization", GetBasicAuthHeader(clientId, clientSecret));

    private async Task<OAuthResponse> InnerExecuteAsync(HttpRequestData data)
        => await _httpClient.ExecuteAsync<OAuthResponse>(data);

    private string GetBasicAuthHeader(string clientId, string clientSecret)
        => $"Basic {Encoder.Encode64($"{clientId}:{clientSecret}")}";
}