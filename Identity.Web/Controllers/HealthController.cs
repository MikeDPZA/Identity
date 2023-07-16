using Microsoft.AspNetCore.Mvc;

namespace Identity.Web.Controllers;

[ApiController]
[Route("v1/Health")]
public class HealthController: ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Ping()
    {
        _logger.Log(LogLevel.Information, "{time}:Pong", DateTime.Now.ToString());
        return Ok("Pong");
    }
}