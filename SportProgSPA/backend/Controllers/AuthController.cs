using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProg.Api.Data;
using SportProg.Api.Dtos;
using SportProg.Api.Models;
using SportProg.Api.Services;

namespace SportProg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SportProgDbContext _db;
    private readonly PasswordService _passwords;
    private readonly TokenService _tokens;

    public AuthController(SportProgDbContext db, PasswordService passwords, TokenService tokens)
    {
        _db = db;
        _passwords = passwords;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Заполните имя, email и пароль." });
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(new { message = "Пароль должен быть не короче 6 символов." });
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(user => user.Email == email))
        {
            return Conflict(new { message = "Пользователь с таким email уже существует." });
        }

        var (hash, salt) = _passwords.CreateHash(request.Password);
        var colors = new[] { "#2563eb", "#16a34a", "#dc2626", "#0891b2", "#7c3aed" };
        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            AvatarColor = colors[Math.Abs(email.GetHashCode()) % colors.Length]
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse(_tokens.CreateToken(user), ToProfile(user)));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users
            .Include(item => item.Submissions)
            .ThenInclude(item => item.ChallengeTask)
            .ThenInclude(task => task!.AlgorithmTopic)
            .FirstOrDefaultAsync(item => item.Email == email);

        if (user is null || !_passwords.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Unauthorized(new { message = "Неверный email или пароль." });
        }

        return Ok(new AuthResponse(_tokens.CreateToken(user), ToProfile(user)));
    }

    private static UserProfileDto ToProfile(User user)
    {
        var submissions = user.Submissions
            .OrderByDescending(item => item.SubmittedAt)
            .Take(5)
            .Select(item => new RecentSubmissionDto(
                item.Id,
                item.ChallengeTask?.Title ?? "Задача",
                item.ChallengeTask?.AlgorithmTopic?.Slug ?? "",
                item.Status,
                item.Language,
                item.Points,
                item.SubmittedAt))
            .ToList();

        return new UserProfileDto(
            user.Id,
            user.Name,
            user.Email,
            user.City,
            user.AvatarColor,
            user.Rating,
            user.SolvedCount,
            user.RegisteredAt,
            submissions);
    }
}
