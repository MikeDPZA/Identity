using Identity.Shared.Models;
using Identity.Shared.Models.OAuth;

namespace Identity.Shared.Interfaces;

public interface IOAuthFacade
{
    Task<OAuthResponse> GetAccessTokenAsync(string code);
    Task<OAuthResponse> RefreshTokenAsync(string refreshToken);
    Task<OAuthResponse> GetClientCredentialsTokenAsync();
    Uri GetLoginForm();
}