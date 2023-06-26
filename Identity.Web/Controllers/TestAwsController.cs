using Identity.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Web.Controllers;

[Route("TestAws")]
[Authorize(AuthenticationSchemes = "CC")]
[Authorize(AuthenticationSchemes = "UC")]
public class TestAwsController: ControllerBase
{
    private readonly IOAuthFacade _oAuthFacade;

    public TestAwsController(IOAuthFacade oAuthFacade)
    {
        _oAuthFacade = oAuthFacade;
    }
    
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        
        var x = await _oAuthFacade.GetAccessTokenAsync("309ae362-42f2-4754-bde1-ee86aadd85c8", "https://localhost:5001/signin-oidc");
        return Ok(x);
    }
}