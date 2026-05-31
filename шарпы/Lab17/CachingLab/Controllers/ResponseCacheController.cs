using Microsoft.AspNetCore.Mvc;

namespace CachingLab.Controllers;

[ApiController]
[Route("api/response-cache")]
public class ResponseCacheController : ControllerBase
{
    [HttpGet("time")]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, NoStore = false)]
    public IActionResult GetCachedTime()
    {
        return Ok(new
        {
            message = "Этот ответ может кэшироваться 30 секунд",
            generatedAtUtc = DateTime.UtcNow,
            randomValue = Random.Shared.Next(1, 100000)
        });
    }

    [HttpGet("time-by-category")]
    [ResponseCache(
        Duration = 30,
        Location = ResponseCacheLocation.Any,
        VaryByQueryKeys = new[] { "category" })]
    public IActionResult GetCachedTimeByCategory([FromQuery] string category = "default")
    {
        return Ok(new
        {
            message = "Ответ кэшируется отдельно для разных значений query-параметра category",
            category,
            generatedAtUtc = DateTime.UtcNow,
            randomValue = Random.Shared.Next(1, 100000)
        });
    }

    [HttpGet("no-cache")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult GetWithoutCache()
    {
        return Ok(new
        {
            message = "Этот ответ не должен кэшироваться",
            generatedAtUtc = DateTime.UtcNow,
            randomValue = Random.Shared.Next(1, 100000)
        });
    }
}