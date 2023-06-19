using System.Globalization;

namespace Identity.Cognito.Models.Config;

public class CognitoConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string UserPoolId { get; set; }
    public string Domain { get; set; }
    public RegionInfo Region { get; set; }
    public Uri BaseUrl => new Uri($"https://{Domain}.auth.{Region}.amazoncognito.com/");
    public Uri RedirectUri { get; set; }
}