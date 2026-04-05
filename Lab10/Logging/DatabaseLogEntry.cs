namespace Lab10.Logging;

public sealed class DatabaseLogEntry
{
    public long Id { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string Level { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public int EventId { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Exception { get; set; }

    public string? Scope { get; set; }
}
