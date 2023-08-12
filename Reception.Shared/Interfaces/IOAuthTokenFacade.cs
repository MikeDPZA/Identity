using Reception.Shared.Models.OAuth;

namespace Reception.Shared.Interfaces;

public interface IOAuthTokenFacade
{
    Task<OAuthResponse> GetAccessTokenAsync(string code);
    Task<OAuthResponse> RefreshTokenAsync(string refreshToken);
    Task<OAuthResponse> GetClientCredentialsTokenAsync();
    Uri GetLoginUri(string? state);
    Uri GetLogoutUri(string? accessToken);
}