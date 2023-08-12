using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Reception.Shared.Constants;

namespace Reception.Shared.Attributes;

public class ClientCredentialsAuthAttribute: AuthorizeAttribute
{
    public ClientCredentialsAuthAttribute():base(AuthenticationSchemeConstants.ClientCredentials)
    {
        
    }
}