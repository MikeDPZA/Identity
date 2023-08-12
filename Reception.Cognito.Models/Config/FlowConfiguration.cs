using System.Diagnostics.CodeAnalysis;

namespace Reception.Cognito.Models.Config;

[ExcludeFromCodeCoverage]
public class FlowConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string[] Scopes { get; set; }
    public Uri RedirectUri { get; set; }
}