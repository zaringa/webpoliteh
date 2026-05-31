namespace SportProg.Api.Models;

public class FavoriteAlgorithm
{
    public int UserId { get; set; }
    public int AlgorithmTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public AlgorithmTopic? AlgorithmTopic { get; set; }
}
