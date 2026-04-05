namespace Lab10.Logging;

public sealed class DatabaseLoggerOptions
{
    public string ConnectionString { get; set; } = "Data Source=Logs/lab10-logs.db";

    public string TableName { get; set; } = "ApplicationLogs";

    public LogLevel MinLevel { get; set; } = LogLevel.Warning;
}
