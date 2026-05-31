namespace SportProg.Api.Models;

public class CollectionItem
{
    public int LearningCollectionId { get; set; }
    public int AlgorithmTopicId { get; set; }
    public int Position { get; set; }

    public LearningCollection? LearningCollection { get; set; }
    public AlgorithmTopic? AlgorithmTopic { get; set; }
}
