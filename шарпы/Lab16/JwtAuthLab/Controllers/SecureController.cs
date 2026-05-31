using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;  // ← для Select

namespace JwtAuthLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok(new { message = "Публичный ресурс. Токен не требуется." });
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok(new
        {
            message = "Профиль доступен любому аутентифицированному пользователю.",
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            login = User.FindFirstValue(ClaimTypes.Name),
            role = User.FindFirstValue(ClaimTypes.Role),
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new { message = "Этот ресурс доступен только пользователю с ролью Admin." });
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("manager-or-admin")]
    public IActionResult ManagerOrAdmin()
    {
        return Ok(new { message = "Этот ресурс доступен пользователям с ролью Manager или Admin." });
    }

    [Authorize(Roles = "User,Manager,Admin")]
    [HttpGet("any-role")]
    public IActionResult AnyKnownRole()
    {
        return Ok(new { message = "Этот ресурс доступен пользователям с ролью User, Manager или Admin." });
    }
}