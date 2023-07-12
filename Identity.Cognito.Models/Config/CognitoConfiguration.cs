using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Identity.Cognito.Models.Config;

[ExcludeFromCodeCoverage]
public class CognitoConfiguration
{
    public string UserPoolId { get; set; }
    public string Domain { get; set; }
    public string Region { get; set; }
    public Uri BaseUrl => new Uri($"https://{Domain}.auth.{Region}.amazoncognito.com");
    public Uri AuthorityUrl => new Uri($"https://cognito-idp.{Region}.amazonaws.com/{UserPoolId}");
    public FlowConfiguration ClientCredentialsFlow { get; set; }
    public FlowConfiguration UserCredentialsFlow { get; set; }
}