using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Reception.Shared.Models.OAuth;

[ExcludeFromCodeCoverage]
public class OAuthResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}