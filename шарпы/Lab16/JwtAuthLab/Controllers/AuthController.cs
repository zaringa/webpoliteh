using JwtAuthLab.Models;
using JwtAuthLab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;  // ← для List<>
using System.Linq;                 // ← для FirstOrDefault
using System;                              // StringComparison

namespace JwtAuthLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    private static readonly List<AppUser> Users = new()
    {
        new AppUser(Id: "1", Login: "admin", Password: "admin123", Role: "Admin"),
        new AppUser(Id: "2", Login: "user", Password: "user123", Role: "User"),
        new AppUser(Id: "3", Login: "manager", Password: "manager123", Role: "Manager")
    };

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var user = Users.FirstOrDefault(u =>
            u.Login.Equals(request.Login, StringComparison.OrdinalIgnoreCase)
            && u.Password == request.Password);

        if (user is null)
        {
            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        var token = _jwtService.CreateToken(user);

        return Ok(new LoginResponse(
            Token: token,
            UserId: user.Id,
            Login: user.Login,
            Role: user.Role));
    }
}