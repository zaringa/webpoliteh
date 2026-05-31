using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProg.Api.Data;
using SportProg.Api.Dtos;
using SportProg.Api.Models;
using SportProg.Api.Services;

namespace SportProg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly SportProgDbContext _db;
    private readonly TokenService _tokens;

    public TasksController(SportProgDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? difficulty,
        [FromQuery] string? topic)
    {
        var query = _db.ChallengeTasks
            .Include(task => task.AlgorithmTopic)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim().ToLowerInvariant();
            query = query.Where(task =>
                task.Title.ToLower().Contains(value) ||
                task.Statement.ToLower().Contains(value) ||
                task.ExternalId.ToLower().Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            query = query.Where(task => task.Difficulty == difficulty);
        }

        if (!string.IsNullOrWhiteSpace(topic))
        {
            query = query.Where(task => task.AlgorithmTopic != null && task.AlgorithmTopic.Slug == topic);
        }

        var tasks = await query
            .OrderBy(task => task.Difficulty)
            .ThenByDescending(task => task.SolvedCount)
            .ToListAsync();

        return Ok(tasks.Select(AlgorithmsController.ToTaskDto).ToList());
    }

    [HttpPost("{id:int}/submit")]
    public async Task<IActionResult> Submit(int id, SubmitSolutionRequest request)
    {
        if (!_tokens.TryGetUserId(Request, out var userId))
        {
            return Unauthorized(new { message = "Войдите в аккаунт, чтобы отправлять решения." });
        }

        var task = await _db.ChallengeTasks.FindAsync(id);
        var user = await _db.Users.FindAsync(userId);
        if (task is null || user is null)
        {
            return NotFound(new { message = "Задача или пользователь не найдены." });
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(new { message = "Добавьте код решения." });
        }

        var accepted = request.Code.Trim().Length >= 80 &&
            !request.Code.Contains("todo", StringComparison.OrdinalIgnoreCase);

        var alreadySolved = await _db.Submissions.AnyAsync(submission =>
            submission.UserId == userId &&
            submission.ChallengeTaskId == id &&
            submission.Status == "Accepted");

        var submission = new Submission
        {
            UserId = userId,
            ChallengeTaskId = id,
            Language = string.IsNullOrWhiteSpace(request.Language) ? "C++17" : request.Language.Trim(),
            Status = accepted ? "Accepted" : "Needs review",
            Points = accepted ? 100 : 35
        };

        _db.Submissions.Add(submission);

        if (accepted && !alreadySolved)
        {
            user.SolvedCount += 1;
            user.Rating += 18;
            task.SolvedCount += 1;
            task.AcceptanceRate = Math.Min(99, task.AcceptanceRate + 0.3);
        }

        await _db.SaveChangesAsync();

        return Ok(new
        {
            submission.Id,
            submission.Status,
            submission.Points,
            user.Rating,
            user.SolvedCount
        });
    }
}
