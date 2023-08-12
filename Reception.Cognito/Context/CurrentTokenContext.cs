using System.Security.Claims;

namespace Reception.Cognito.Context;

public class CurrentTokenContext
{
    public CurrentTokenContext(Dictionary<string,string> claims)
    {
        _claimsMap = claims;
        if (claims.TryGetValue(ClaimTypes.NameIdentifier, out var userId) && !string.IsNullOrEmpty(userId))
        {
            UserId = userId;
        }
        if (claims.TryGetValue("client_id", out var clientId) && !string.IsNullOrEmpty(clientId))
        {
            ClientId = clientId;
        }
    }

    private readonly IDictionary<string, string> _claimsMap;
    public string ClientId { get; init; } = "";
    public string UserId { get; init; } = "";
}