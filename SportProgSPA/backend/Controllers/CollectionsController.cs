using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProg.Api.Data;
using SportProg.Api.Dtos;
using SportProg.Api.Services;

namespace SportProg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly SportProgDbContext _db;
    private readonly TokenService _tokens;

    public CollectionsController(SportProgDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CollectionDto>>> GetAll()
    {
        var favorites = await GetFavoriteIdsAsync();
        var collections = await _db.LearningCollections
            .Include(collection => collection.Items)
            .ThenInclude(item => item.AlgorithmTopic)
            .ThenInclude(topic => topic!.Tasks)
            .OrderBy(collection => collection.Id)
            .ToListAsync();

        return Ok(collections.Select(collection => ToDto(collection, favorites)).ToList());
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CollectionDto>> GetBySlug(string slug)
    {
        var favorites = await GetFavoriteIdsAsync();
        var collection = await _db.LearningCollections
            .Include(item => item.Items)
            .ThenInclude(item => item.AlgorithmTopic)
            .ThenInclude(topic => topic!.Tasks)
            .FirstOrDefaultAsync(item => item.Slug == slug);

        if (collection is null)
        {
            return NotFound(new { message = "Подборка не найдена." });
        }

        return Ok(ToDto(collection, favorites));
    }

    private static CollectionDto ToDto(Models.LearningCollection collection, HashSet<int> favorites)
    {
        var topics = collection.Items
            .OrderBy(item => item.Position)
            .Where(item => item.AlgorithmTopic is not null)
            .Select(item => AlgorithmsController.ToDto(item.AlgorithmTopic!, favorites.Contains(item.AlgorithmTopicId), includeTasks: false))
            .ToList();

        return new CollectionDto(
            collection.Id,
            collection.Slug,
            collection.Title,
            collection.Description,
            collection.Level,
            topics);
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
