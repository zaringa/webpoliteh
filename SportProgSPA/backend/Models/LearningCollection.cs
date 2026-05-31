namespace SportProg.Api.Models;

public class LearningCollection
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;

    public ICollection<CollectionItem> Items { get; set; } = new List<CollectionItem>();
}
