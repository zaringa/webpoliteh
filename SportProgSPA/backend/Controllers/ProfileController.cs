using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProg.Api.Data;
using SportProg.Api.Dtos;
using SportProg.Api.Models;
using SportProg.Api.Services;

namespace SportProg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly SportProgDbContext _db;
    private readonly TokenService _tokens;

    public ProfileController(SportProgDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe()
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new { message = "Войдите в аккаунт." });
        }

        return Ok(ToProfile(user));
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMe(UpdateProfileRequest request)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new { message = "Войдите в аккаунт." });
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            user.City = request.City.Trim();
        }

        await _db.SaveChangesAsync();
        return Ok(ToProfile(user));
    }

    [HttpGet("rating")]
    public async Task<ActionResult<IReadOnlyList<UserProfileDto>>> GetRating()
    {
        var users = await _db.Users
            .Include(user => user.Submissions)
            .ThenInclude(submission => submission.ChallengeTask)
            .ThenInclude(task => task!.AlgorithmTopic)
            .OrderByDescending(user => user.Rating)
            .ThenByDescending(user => user.SolvedCount)
            .Take(30)
            .ToListAsync();

        return Ok(users.Select(ToProfile).ToList());
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        if (!_tokens.TryGetUserId(Request, out var userId))
        {
            return null;
        }

        return await _db.Users
            .Include(user => user.Submissions)
            .ThenInclude(submission => submission.ChallengeTask)
            .ThenInclude(task => task!.AlgorithmTopic)
            .FirstOrDefaultAsync(user => user.Id == userId);
    }

    private static UserProfileDto ToProfile(User user)
    {
        var submissions = user.Submissions
            .OrderByDescending(item => item.SubmittedAt)
            .Take(8)
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
