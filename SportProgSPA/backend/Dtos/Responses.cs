namespace SportProg.Api.Dtos;

public record AuthResponse(string Token, UserProfileDto User);

public record UserProfileDto(
    int Id,
    string Name,
    string Email,
    string City,
    string AvatarColor,
    int Rating,
    int SolvedCount,
    DateTime RegisteredAt,
    IReadOnlyList<RecentSubmissionDto> RecentSubmissions);

public record RecentSubmissionDto(
    int Id,
    string TaskTitle,
    string TopicSlug,
    string Status,
    string Language,
    int Points,
    DateTime SubmittedAt);

public record AlgorithmDto(
    int Id,
    string Slug,
    string Title,
    string Category,
    string Difficulty,
    string Summary,
    string Theory,
    string Complexity,
    string[] Tags,
    int Popularity,
    bool IsFavorite,
    IReadOnlyList<TaskDto> Tasks);

public record TaskDto(
    int Id,
    string Title,
    string Difficulty,
    string Statement,
    string InputFormat,
    string OutputFormat,
    string ExampleInput,
    string ExampleOutput,
    string ExternalId,
    int SolvedCount,
    double AcceptanceRate,
    string TopicSlug,
    string TopicTitle);

public record CollectionDto(
    int Id,
    string Slug,
    string Title,
    string Description,
    string Level,
    IReadOnlyList<AlgorithmDto> Topics);
