using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProg.Api.Data;
using SportProg.Api.Dtos;
using SportProg.Api.Models;
using SportProg.Api.Services;

namespace SportProg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlgorithmsController : ControllerBase
{
    private readonly SportProgDbContext _db;
    private readonly TokenService _tokens;

    public AlgorithmsController(SportProgDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AlgorithmDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? difficulty,
        [FromQuery] string? category,
        [FromQuery] string? tag)
    {
        var query = _db.AlgorithmTopics
            .Include(topic => topic.Tasks)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim().ToLowerInvariant();
            query = query.Where(topic =>
                topic.Title.ToLower().Contains(value) ||
                topic.Summary.ToLower().Contains(value) ||
                topic.Tags.ToLower().Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            query = query.Where(topic => topic.Difficulty == difficulty);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(topic => topic.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var value = tag.Trim().ToLowerInvariant();
            query = query.Where(topic => topic.Tags.ToLower().Contains(value));
        }

        var favorites = await GetFavoriteIdsAsync();
        var topics = await query
            .OrderByDescending(topic => topic.Popularity)
            .ThenBy(topic => topic.Title)
            .ToListAsync();

        return Ok(topics.Select(topic => ToDto(topic, favorites.Contains(topic.Id), includeTasks: false)).ToList());
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<AlgorithmDto>> GetBySlug(string slug)
    {
        var topic = await _db.AlgorithmTopics
            .Include(item => item.Tasks)
            .FirstOrDefaultAsync(item => item.Slug == slug);

        if (topic is null)
        {
            return NotFound(new { message = "Тема не найдена." });
        }

        var favorites = await GetFavoriteIdsAsync();
        return Ok(ToDto(topic, favorites.Contains(topic.Id), includeTasks: true));
    }

    internal static AlgorithmDto ToDto(AlgorithmTopic topic, bool isFavorite, bool includeTasks)
    {
        var tasks = includeTasks
            ? topic.Tasks.OrderBy(task => task.Difficulty).Select(ToTaskDto).ToList()
            : new List<TaskDto>();

        return new AlgorithmDto(
            topic.Id,
            topic.Slug,
            topic.Title,
            topic.Category,
            topic.Difficulty,
            topic.Summary,
            topic.Theory,
            topic.Complexity,
            topic.Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
            topic.Popularity,
            isFavorite,
            tasks);
    }

    internal static TaskDto ToTaskDto(ChallengeTask task)
    {
        return new TaskDto(
            task.Id,
            task.Title,
            task.Difficulty,
            task.Statement,
            task.InputFormat,
            task.OutputFormat,
            task.ExampleInput,
            task.ExampleOutput,
            task.ExternalId,
            task.SolvedCount,
            task.AcceptanceRate,
            task.AlgorithmTopic?.Slug ?? "",
            task.AlgorithmTopic?.Title ?? "");
    }

    private async Task<HashSet<int>> GetFavoriteIdsAsync()
    {
        if (!_tokens.TryGetUserId(Request, out var userId))
        {
            return new HashSet<int>();
        }

        var ids = await _db.FavoriteAlgorithms
            .Where(favorite => favorite.UserId == userId)
            .Select(favorite => favorite.AlgorithmTopicId)
            .ToListAsync();

        return ids.ToHashSet();
    }
}
