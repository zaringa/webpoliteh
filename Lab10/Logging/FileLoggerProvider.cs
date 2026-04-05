using Microsoft.Extensions.Options;

namespace Lab10.Logging;

public sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly FileLoggerOptions _options;
    private readonly object _syncRoot = new();
    private readonly StreamWriter _writer;
    private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

    public FileLoggerProvider(IOptions<FileLoggerOptions> options, IWebHostEnvironment environment)
    {
        _options = options.Value;

        var path = _options.Path;
        if (!System.IO.Path.IsPathRooted(path))
        {
            path = System.IO.Path.Combine(environment.ContentRootPath, path);
        }

        var directory = System.IO.Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _writer = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
        {
            AutoFlush = true
        };
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, this);
    }

    internal bool IsEnabled(LogLevel level)
    {
        return level >= _options.MinLevel;
    }

    internal string? BuildScope()
    {
        if (!_options.IncludeScopes)
        {
            return null;
        }

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

    internal void Write(FileLogEntry entry)
    {
        var line =
            $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} | {entry.Level,-11} | {entry.Category} | EventId={entry.EventId.Id} | {entry.Message}";

        if (!string.IsNullOrWhiteSpace(entry.Scope))
        {
            line += $" | Scope={entry.Scope}";
        }

        if (!string.IsNullOrWhiteSpace(entry.Exception))
        {
            var normalizedException = entry.Exception
                .Replace("\r", " ")
                .Replace("\n", " ");
            line += $" | Exception={normalizedException}";
        }

        lock (_syncRoot)
        {
            _writer.WriteLine(line);
        }
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            _writer.Dispose();
        }
    }
}
