using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using LangChain.Databases.JsonConverters;
using Npgsql;
using NpgsqlTypes;
using Pgvector;

namespace LangChain.Databases.Postgres;

/// <summary>
/// 
/// </summary>
[RequiresDynamicCode("Requires dynamic code.")]
[RequiresUnreferencedCode("Requires unreferenced code.")]
public class PostgresDbClient
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schema;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectionString">connection string</param>
    /// <param name="schema">schema name</param>
    public PostgresDbClient(string connectionString, string schema)
    {
        var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        var connection = dataSource.OpenConnection();
        using (connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "CREATE EXTENSION IF NOT EXISTS vector";

            command.ExecuteNonQuery();
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseVector();

        _dataSource = dataSourceBuilder.Build();
        _schema = schema;

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new ObjectAsPrimitiveConverter(
                    floatFormat: FloatFormat.Double,
                    unknownNumberFormat: UnknownNumberFormat.Error,
                    objectFormat: ObjectFormat.Expando)
            },
            WriteIndented = true,
        };
    }

    /// <summary>
    /// Check if table exists
    /// </summary>
    /// <param name="tableName">name of the table</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    public async Task<bool> IsTableExistsAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            var tablesSchema = await connection
                .GetSchemaAsync("TABLES", [string.Empty, "public", tableName], cancellationToken)
                .ConfigureAwait(false);

            return tablesSchema.Rows.Count != 0;
        }
    }


    public async Task<IReadOnlyList<string>> ListTablesAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            var tablesSchema = await connection
                .GetSchemaAsync("TABLES", [string.Empty, "public"], cancellationToken)
                .ConfigureAwait(false);

            return tablesSchema.Rows.Cast<DataRow>()
                .Select(row => row["TABLE_NAME"].ToString() ?? string.Empty)
                .ToList();
        }
    }

    /// <summary>
    /// Create table for documents with embeddings
    /// </summary>
    /// <param name="tableName">name of the table</param>
    /// <param name="dimensions"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public async Task CreateEmbeddingTableAsync(string tableName, int dimensions, CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            var name = GetFullTableName(tableName);

            cmd.CommandText = $@"
CREATE TABLE IF NOT EXISTS {name}
(
    id TEXT NOT NULL,
    content TEXT,
    metadata JSONB,
    embedding vector({dimensions}),
    timestamp TIMESTAMP WITH TIME ZONE,
    PRIMARY KEY (id)
);";

            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Drop table
    /// </summary>
    /// <param name="tableName">name of the table</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public async Task DropTableAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"DROP TABLE IF EXISTS {GetFullTableName(tableName)}";

            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Insert or update existing record
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="id"></param>
    /// <param name="content"></param>
    /// <param name="metadata"></param>
    /// <param name="embedding"></param>
    /// <param name="timestamp"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public async Task UpsertAsync(
        string tableName,
        string id,
        string content,
        IReadOnlyDictionary<string, object>? metadata,
        ReadOnlyMemory<float>? embedding,
        DateTime? timestamp,
        CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();

            cmd.CommandText = $@"
INSERT INTO {GetFullTableName(tableName)} (id, content, metadata, embedding, timestamp)
VALUES(@id, @content, @metadata, @embedding, @timestamp)
ON CONFLICT (id)
DO UPDATE SET content=@content, metadata=@metadata, embedding=@embedding, timestamp=@timestamp;";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@content", content);

            var metadataString = metadata != null
                ? JsonSerializer.Serialize(metadata, _jsonSerializerOptions)
                : (object)DBNull.Value;
            cmd.Parameters.AddWithValue("@metadata", NpgsqlDbType.Jsonb, metadataString);

            var vector = embedding != null ? new Pgvector.Vector(embedding.Value) : (object)DBNull.Value;
            cmd.Parameters.AddWithValue("@embedding", vector);
            cmd.Parameters.AddWithValue("@timestamp", NpgsqlDbType.TimestampTz, timestamp ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Get record by id
    /// </summary>
    [CLSCompliant(false)]
    public async Task<EmbeddingTableRecord?> GetRecordByIdAsync(
        string tableName,
        string id,
        bool withEmbeddings = false,
        CancellationToken cancellationToken = default)
    {
        var fullTableName = GetFullTableName(tableName);
        var queryColumns = withEmbeddings
            ? "id, content, metadata, timestamp"
            : "id, content, metadata, timestamp, embedding";

        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @$"
SELECT {queryColumns}
FROM {fullTableName}
WHERE id = @id";

            cmd.Parameters.AddWithValue("@id", id);

            var dataReader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            await using (dataReader)
            {
                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var record = await ReadEntryAsync(dataReader, withEmbeddings, cancellationToken)
                        .ConfigureAwait(false);

                    return record;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Get nearest matches for embedding
    /// </summary>
    /// <param name="tableName">table where to search</param>
    /// <param name="embedding">embedding to compare with</param>
    /// <param name="strategy">distance strategy</param>
    /// <param name="limit">limit results</param>
    /// <param name="minRelevanceScore">min score for results</param>
    /// <param name="withEmbeddings">include or not embeddings in the result</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public async Task<IEnumerable<(EmbeddingTableRecord, float)>> GetWithDistanceAsync(
        string tableName, float[] embedding, DistanceStrategy strategy,
        int limit, double minRelevanceScore = 0, bool withEmbeddings = false,
        CancellationToken cancellationToken = default)
    {
        var fullTableName = GetFullTableName(tableName);
        var queryColumns = withEmbeddings
            ? "id, content, metadata, timestamp"
            : "id, content, metadata, timestamp, embedding";

        var distanceExpression = strategy switch
        {
            DistanceStrategy.Euclidean => "(embedding <-> @embedding)",
            DistanceStrategy.Cosine => "(embedding <=> @embedding)",
            DistanceStrategy.InnerProduct => "(embedding <#> @embedding) * -1",
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };

        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @$"
SELECT *
FROM (
    SELECT {queryColumns}, 1 - {distanceExpression} AS score
    FROM {fullTableName}
    ) AS score
WHERE score >= @min_relevance_score
ORDER BY score DESC
LIMIT @limit";

            cmd.Parameters.AddWithValue("@embedding", new Pgvector.Vector(embedding));
            cmd.Parameters.AddWithValue("@collection", tableName);
            cmd.Parameters.AddWithValue("@min_relevance_score", minRelevanceScore);
            cmd.Parameters.AddWithValue("@limit", limit);

            var dataReader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            var result = new List<(EmbeddingTableRecord, float)>((int)dataReader.Rows);
            await using (dataReader)
            {
                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var score = dataReader.GetFloat(dataReader.GetOrdinal("score"));
                    var record = await ReadEntryAsync(dataReader, withEmbeddings, cancellationToken)
                        .ConfigureAwait(false);
                    result.Add((record, score));
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Fetch record
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="id"></param>
    /// <param name="withEmbeddings">include or not embeddings in the result</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public async Task<EmbeddingTableRecord?> ReadAsync(
        string tableName, string id,
        bool withEmbeddings = false,
        CancellationToken cancellationToken = default)
    {
        var fullTableName = GetFullTableName(tableName);
        var queryColumns = withEmbeddings
            ? "id, content, metadata, timestamp"
            : "id, content, metadata, timestamp, embedding";

        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT {queryColumns} FROM {fullTableName} WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);

            var dataReader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            await using (dataReader)
            {
                if (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await ReadEntryAsync(dataReader, withEmbeddings, cancellationToken).ConfigureAwait(false);
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Read records batch
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="ids"></param>
    /// <param name="withEmbeddings">include or not embeddings in the result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public async IAsyncEnumerable<EmbeddingTableRecord> ReadBatchAsync(
        string tableName,
        IReadOnlyList<string> ids,
        bool withEmbeddings = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ids = ids ?? throw new ArgumentNullException(nameof(ids));

        if (ids.Count == 0)
        {
            yield break;
        }

        var fullTableName = GetFullTableName(tableName);
        var queryColumns = withEmbeddings
            ? "id, content, metadata, timestamp"
            : "id, content, metadata, timestamp, embedding";

        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @$"
SELECT {queryColumns}
FROM {fullTableName}
WHERE id = ANY(@ids)";

            cmd.Parameters.AddWithValue("@ids", NpgsqlDbType.Array | NpgsqlDbType.Text, ids);

            using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return await ReadEntryAsync(dataReader, withEmbeddings, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Delete record by id
    /// </summary>
    /// <param name="tableName">embeddings table</param>
    /// <param name="id">id of the record to delete</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public async Task DeleteAsync(string tableName, string id, CancellationToken cancellationToken = default)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {GetFullTableName(tableName)} WHERE key=@id";
            cmd.Parameters.AddWithValue("@id", id);

            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Delete records by list of ids
    /// </summary>
    /// <param name="tableName">embeddings table</param>
    /// <param name="ids">list of record ids to delete</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public async Task DeleteBatchAsync(
        string tableName,
        IReadOnlyList<string> ids,
        CancellationToken cancellationToken = default)
    {
        ids = ids ?? throw new ArgumentNullException(nameof(ids));

        if (ids.Count == 0)
        {
            return;
        }

        var connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @$"
DELETE FROM {GetFullTableName(tableName)}
WHERE id = ANY(@ids)";

            cmd.Parameters.AddWithValue("@ids", NpgsqlDbType.Array | NpgsqlDbType.Text, ids);

            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Read a entry.
    /// </summary>
    /// <param name="dataReader">The <see cref="NpgsqlDataReader"/> to read.</param>
    /// <param name="withEmbeddings">include or not embeddings in the result</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    private async Task<EmbeddingTableRecord> ReadEntryAsync(
        NpgsqlDataReader dataReader, bool withEmbeddings = false,
        CancellationToken cancellationToken = default)
    {
        var id = dataReader.GetString(dataReader.GetOrdinal("id"));
        var content = dataReader.GetString(dataReader.GetOrdinal("content"));

        var isMetadataNull = await dataReader.IsDBNullAsync(dataReader.GetOrdinal("metadata"), cancellationToken).ConfigureAwait(false);
        var metadataRaw = isMetadataNull
            ? null
            : dataReader.GetString(dataReader.GetOrdinal("metadata"));

        var metadata = metadataRaw != null
            ? JsonSerializer.Deserialize<Dictionary<string, object>>(metadataRaw, _jsonSerializerOptions)
            : null;

        var timestamp = await dataReader
            .GetFieldValueAsync<DateTime?>(dataReader.GetOrdinal("timestamp"), cancellationToken)
            .ConfigureAwait(false);

        Pgvector.Vector? embedding = null;
        if (withEmbeddings)
        {
            embedding = await dataReader
                .GetFieldValueAsync<Pgvector.Vector>(dataReader.GetOrdinal("embedding"), cancellationToken)
                .ConfigureAwait(false);
        }

        return new EmbeddingTableRecord(id, content, metadata, embedding, timestamp);
    }

    /// <summary>
    /// Get full table name with schema from table name.
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    private string GetFullTableName(string tableName) => $"{_schema}.\"{tableName}\"";
}

/// <summary>
/// Document with embedding db record
/// </summary>
[CLSCompliant(false)]
public record EmbeddingTableRecord(
    string Id,
    string Content,
    Dictionary<string, object>? Metadata,
    Pgvector.Vector? Embedding,
    DateTime? DateTime);