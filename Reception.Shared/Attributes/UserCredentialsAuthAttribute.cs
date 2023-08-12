using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Reception.Shared.Constants;

namespace Reception.Shared.Attributes;

public class UserCredentialsAuthAttribute: AuthorizeAttribute
{
    public UserCredentialsAuthAttribute():base(AuthenticationSchemeConstants.UserCredentials)
    {
    }
}