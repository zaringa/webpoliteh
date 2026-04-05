using Microsoft.Extensions.Options;

namespace Lab10.Logging;

public sealed class DatabaseLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly DatabaseLoggerOptions _options;
    private readonly DatabaseLogStore _store;
    private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

    public DatabaseLoggerProvider(IOptions<DatabaseLoggerOptions> options, DatabaseLogStore store)
    {
        _options = options.Value;
        _store = store;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(categoryName, this);
    }

    internal bool IsEnabled(LogLevel level)
    {
        return level >= _options.MinLevel;
    }

    internal string? BuildScope()
    {
        var scopes = new List<string>();
        _scopeProvider.ForEachScope((scope, state) => state.Add(scope?.ToString() ?? string.Empty), scopes);

        if (scopes.Count == 0)
        {
            return null;
        }

        return string.Join(" => ", scopes);
    }

    internal IDisposable PushScope<TState>(TState state) where TState : notnull
    {
        return _scopeProvider.Push(state);
    }

    internal void Write(DatabaseLogEntry entry)
    {
        _store.Write(entry);
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public void Dispose()
    {
    }
}
