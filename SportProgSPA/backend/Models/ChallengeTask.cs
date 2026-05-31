namespace SportProg.Api.Models;

public class ChallengeTask
{
    public int Id { get; set; }
    public int AlgorithmTopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public string InputFormat { get; set; } = string.Empty;
    public string OutputFormat { get; set; } = string.Empty;
    public string ExampleInput { get; set; } = string.Empty;
    public string ExampleOutput { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public int SolvedCount { get; set; }
    public double AcceptanceRate { get; set; }

    public AlgorithmTopic? AlgorithmTopic { get; set; }
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
