using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Lab10.Logging;

public sealed class DatabaseLogStore
{
    private readonly string _connectionString;
    private readonly string _tableName;
    private readonly object _syncRoot = new();

    public DatabaseLogStore(IOptions<DatabaseLoggerOptions> options, IWebHostEnvironment environment)
    {
        var currentOptions = options.Value;
        _tableName = currentOptions.TableName;

        var builder = new SqliteConnectionStringBuilder(currentOptions.ConnectionString);
        if (!string.IsNullOrWhiteSpace(builder.DataSource) && !System.IO.Path.IsPathRooted(builder.DataSource))
        {
            builder.DataSource = System.IO.Path.Combine(environment.ContentRootPath, builder.DataSource);
        }

        var directory = System.IO.Path.GetDirectoryName(builder.DataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _connectionString = builder.ConnectionString;

        EnsureDatabase();
    }

    private void EnsureDatabase()
    {
        lock (_syncRoot)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"""
                CREATE TABLE IF NOT EXISTS [{_tableName}] (
                    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [Timestamp] TEXT NOT NULL,
                    [Level] TEXT NOT NULL,
                    [Category] TEXT NOT NULL,
                    [EventId] INTEGER NOT NULL,
                    [Message] TEXT NOT NULL,
                    [Exception] TEXT NULL,
                    [Scope] TEXT NULL
                );
                """;
            command.ExecuteNonQuery();
        }
    }

    public void Write(DatabaseLogEntry entry)
    {
        lock (_syncRoot)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"""
                INSERT INTO [{_tableName}] ([Timestamp], [Level], [Category], [EventId], [Message], [Exception], [Scope])
                VALUES ($timestamp, $level, $category, $eventId, $message, $exception, $scope);
                """;
            command.Parameters.AddWithValue("$timestamp", entry.Timestamp.ToString("O"));
            command.Parameters.AddWithValue("$level", entry.Level);
            command.Parameters.AddWithValue("$category", entry.Category);
            command.Parameters.AddWithValue("$eventId", entry.EventId);
            command.Parameters.AddWithValue("$message", entry.Message);
            command.Parameters.AddWithValue("$exception", (object?)entry.Exception ?? DBNull.Value);
            command.Parameters.AddWithValue("$scope", (object?)entry.Scope ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }

    public IReadOnlyList<DatabaseLogEntry> ReadLatest(int take, string? level)
    {
        take = Math.Clamp(take, 1, 200);

        lock (_syncRoot)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = level is null
                ? $"""
                    SELECT [Id], [Timestamp], [Level], [Category], [EventId], [Message], [Exception], [Scope]
                    FROM [{_tableName}]
                    ORDER BY [Id] DESC
                    LIMIT $take;
                    """
                : $"""
                    SELECT [Id], [Timestamp], [Level], [Category], [EventId], [Message], [Exception], [Scope]
                    FROM [{_tableName}]
                    WHERE [Level] = $level
                    ORDER BY [Id] DESC
                    LIMIT $take;
                    """;

            command.Parameters.AddWithValue("$take", take);
            if (level is not null)
            {
                command.Parameters.AddWithValue("$level", level);
            }

            var logs = new List<DatabaseLogEntry>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                logs.Add(new DatabaseLogEntry
                {
                    Id = reader.GetInt64(0),
                    Timestamp = DateTimeOffset.Parse(reader.GetString(1)),
                    Level = reader.GetString(2),
                    Category = reader.GetString(3),
                    EventId = reader.GetInt32(4),
                    Message = reader.GetString(5),
                    Exception = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Scope = reader.IsDBNull(7) ? null : reader.GetString(7)
                });
            }

            return logs;
        }
    }
}
