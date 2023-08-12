using Reception.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Reception.Shared.Attributes;

namespace Reception.Web.Controllers;

[ApiController]
[Route("v1/Token")]
public class TokenController : ControllerBase
{
    private readonly IOAuthTokenFacade _oAuthFacade;

    public TokenController(IOAuthTokenFacade oAuthFacade)
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

    [UserCredentialsAuth]
    [HttpGet("Refresh")]
    public async Task<IActionResult> RefreshToken([FromQuery] string refresher)
    {
        try
        {
            var result = await _oAuthFacade.RefreshTokenAsync(refresher);
            return Ok(result);
        }
        catch (Exception e)
        {
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
    public IActionResult GetLoginForm([FromQuery] string? state)
    {
        try
        {
            var result = _oAuthFacade.GetLoginUri(state);
            return Ok(result.ToString());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [UserCredentialsAuth]
    [HttpGet("Logout")]
    public IActionResult Logout([FromQuery] string? state)
    {
        try
        {
            var uri = _oAuthFacade.GetLogoutUri(state);
            return Ok(uri);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}