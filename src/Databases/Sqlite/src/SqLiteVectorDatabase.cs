using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace LangChain.Databases.Sqlite;

/// <summary>
/// 
/// </summary>
public sealed class SqLiteVectorDatabase : IVectorDatabase, IDisposable
{
    private readonly SqliteConnection _connection;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataSource">File path(like vectors.db) or :memory:</param>
    /// <param name="distanceMetrics"></param>
    public SqLiteVectorDatabase(
        string dataSource = ":memory:",
        EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean)
    {
        Func<float[], float[], float> distanceFunction;
        if (distanceMetrics == EDistanceMetrics.Euclidean)
            distanceFunction = Utils.ComputeEuclideanDistance;
        else
            distanceFunction = Utils.ComputeManhattanDistance;

        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

        _connection = new SqliteConnection($"Data Source={dataSource}");
        _connection.Open();
        _connection.CreateFunction(
            "distance",
            (string a, string b)
                =>
            {
                var vecA = JsonSerializer.Deserialize(a, Sqlite.SourceGenerationContext.Default.SingleArray);
                var vecB = JsonSerializer.Deserialize(b, Sqlite.SourceGenerationContext.Default.SingleArray);
                if (vecA == null || vecB == null)
                    return 0f;

                return distanceFunction(vecA, vecB);
            });
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    /// <inheritdoc />
    public async Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException($"Collection '{collectionName}' does not exist.");
        }

        return new SqLiteVectorCollection(_connection, collectionName);
    }

    /// <inheritdoc />
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"DROP TABLE IF EXISTS {collectionName}";

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            await CreateCollectionAsync(collectionName, dimensions, cancellationToken).ConfigureAwait(false);
        }

        return await GetCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{collectionName}'";
        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

        return result != null;
    }

    /// <inheritdoc />
    public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS {collectionName} (id TEXT PRIMARY KEY, vector BLOB, document TEXT)";

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'vectors' AND name NOT LIKE 'sqlite_%';";
        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var collections = new List<string>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            collections.Add(reader["name"].ToString() ?? string.Empty);
        }

        return collections;
    }
}