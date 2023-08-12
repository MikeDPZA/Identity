using Reception.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Reception.Web.Controllers;

[ApiController]
[Route("v1/Token")]
public class TokenController: ControllerBase
{
    private readonly IOAuthFacade _oAuthFacade;

    public TokenController(IOAuthFacade oAuthFacade)
    {
        _oAuthFacade = oAuthFacade;
    }
    
    [HttpGet("Authorization")]
    public async Task<IActionResult> GetAuthorizationToken([FromQuery] string code)
    {
        try
        {
            var result = await _oAuthFacade.GetAccessTokenAsync(code);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is HttpRequestException)
            {
                return BadRequest(e.Message);
            }
            
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("ClientCredentials")]
    public async Task<IActionResult> GetClientCredentialsToken()
    {
        try
        {
            var result = await _oAuthFacade.GetClientCredentialsTokenAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("Login")]
    public IActionResult GetLoginForm([FromQuery] bool redirect = false)
    {
        try
        {
            var result = _oAuthFacade.GetLoginForm();
            if (redirect)
            {
                return Redirect(result.ToString());    
            }

            return Ok(result.ToString());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}