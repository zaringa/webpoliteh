namespace SportProg.Api.Models;

public class Submission
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ChallengeTaskId { get; set; }
    public string Language { get; set; } = "C++17";
    public string Status { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ChallengeTask? ChallengeTask { get; set; }
}
