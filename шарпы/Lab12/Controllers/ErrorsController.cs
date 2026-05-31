using Microsoft.AspNetCore.Mvc;

namespace Lab12.Controllers;

[ApiController]
[Route("api/errors")]
public class ErrorsController : ControllerBase
{
    [HttpGet("unhandled")]
    public IActionResult ThrowUnhandled()
    {
        throw new InvalidOperationException("Unhandled exception demo for Lab12.");
    }
}
