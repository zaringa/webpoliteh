using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProg.Api.Data;
using SportProg.Api.Dtos;
using SportProg.Api.Models;
using SportProg.Api.Services;

namespace SportProg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly SportProgDbContext _db;
    private readonly TokenService _tokens;

    public FavoritesController(SportProgDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AlgorithmDto>>> Get()
    {
        var userId = RequireUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Войдите в аккаунт." });
        }

        var favorites = await _db.FavoriteAlgorithms
            .Where(item => item.UserId == userId)
            .Include(item => item.AlgorithmTopic)
            .ThenInclude(topic => topic!.Tasks)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();

        return Ok(favorites
            .Where(item => item.AlgorithmTopic is not null)
            .Select(item => AlgorithmsController.ToDto(item.AlgorithmTopic!, isFavorite: true, includeTasks: false))
            .ToList());
    }

    [HttpPost("{topicId:int}")]
    public async Task<IActionResult> Add(int topicId)
    {
        var userId = RequireUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Войдите в аккаунт." });
        }

        if (!await _db.AlgorithmTopics.AnyAsync(topic => topic.Id == topicId))
        {
            return NotFound(new { message = "Тема не найдена." });
        }

        var exists = await _db.FavoriteAlgorithms.AnyAsync(item => item.UserId == userId && item.AlgorithmTopicId == topicId);
        if (!exists)
        {
            _db.FavoriteAlgorithms.Add(new FavoriteAlgorithm { UserId = userId.Value, AlgorithmTopicId = topicId });
            await _db.SaveChangesAsync();
        }

        return Ok(new { isFavorite = true });
    }

    [HttpDelete("{topicId:int}")]
    public async Task<IActionResult> Remove(int topicId)
    {
        var userId = RequireUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Войдите в аккаунт." });
        }

        var favorite = await _db.FavoriteAlgorithms.FindAsync(userId.Value, topicId);
        if (favorite is not null)
        {
            _db.FavoriteAlgorithms.Remove(favorite);
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    private int? RequireUserId()
    {
        return _tokens.TryGetUserId(Request, out var userId) ? userId : null;
    }
}
