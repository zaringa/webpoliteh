namespace SportProg.Api.Models;

public class AlgorithmTopic
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Theory { get; set; } = string.Empty;
    public string Complexity { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public int Popularity { get; set; }

    public ICollection<ChallengeTask> Tasks { get; set; } = new List<ChallengeTask>();
    public ICollection<FavoriteAlgorithm> Favorites { get; set; } = new List<FavoriteAlgorithm>();
    public ICollection<CollectionItem> CollectionItems { get; set; } = new List<CollectionItem>();
}
