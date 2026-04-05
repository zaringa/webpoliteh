namespace Lab10.Logging;

internal sealed record FileLogEntry(
    DateTimeOffset Timestamp,
    LogLevel Level,
    string Category,
    EventId EventId,
    string Message,
    string? Exception,
    string? Scope);
