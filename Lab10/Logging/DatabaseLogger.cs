namespace Lab10.Logging;

internal sealed class DatabaseLogger : ILogger
{
    private readonly string _categoryName;
    private readonly DatabaseLoggerProvider _provider;

    public DatabaseLogger(string categoryName, DatabaseLoggerProvider provider)
    {
        _categoryName = categoryName;
        _provider = provider;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return _provider.PushScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _provider.IsEnabled(logLevel);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        if (string.IsNullOrWhiteSpace(message) && exception is null)
        {
            return;
        }

        _provider.Write(new DatabaseLogEntry
        {
            Timestamp = DateTimeOffset.Now,
            Level = logLevel.ToString(),
            Category = _categoryName,
            EventId = eventId.Id,
            Message = message,
            Exception = exception?.ToString(),
            Scope = _provider.BuildScope()
        });
    }
}
