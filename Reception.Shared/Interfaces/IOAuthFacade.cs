using Reception.Shared.Models;
using Reception.Shared.Models.OAuth;

namespace Reception.Shared.Interfaces;

public interface IOAuthFacade
{
    Task<OAuthResponse> GetAccessTokenAsync(string code);
    Task<OAuthResponse> RefreshTokenAsync(string refreshToken);
    Task<OAuthResponse> GetClientCredentialsTokenAsync();
    Uri GetLoginForm();
}