namespace Lab10.Logging;

public sealed class FileLoggerOptions
{
    public string Path { get; set; } = "Logs/lab10-file.log";

    public LogLevel MinLevel { get; set; } = LogLevel.Information;

    public bool IncludeScopes { get; set; }
}
