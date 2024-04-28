using System.Text.Json;
using System.Text.Json.Serialization;
using LangChain.Databases.JsonConverters;
using LangChain.Sources;
using Microsoft.Data.Sqlite;

namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public sealed class SQLiteVectorStore : IVectorDatabase, IDisposable
{
    private readonly string _tableName;
    private readonly Func<float[], float[], float> _distanceFunction;
    private readonly SqliteConnection _connection;

    public static SQLIteVectorStoreOptions DefaultOptions { get; } = new();


    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="tableName"></param>
    /// <param name="distanceMetrics"></param>
    public SQLiteVectorStore(
        string filename,
        string tableName,
        EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean)
    {
        _tableName = tableName;
        if (distanceMetrics == EDistanceMetrics.Euclidean)
            _distanceFunction = Utils.ComputeEuclideanDistance;
        else
            _distanceFunction = Utils.ComputeManhattanDistance;

        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

        _connection = new SqliteConnection($"Data Source={filename}");
        _connection.Open();
        _connection.CreateFunction(
            "distance",
            (string a, string b)
                =>
            {
                var vecA = JsonSerializer.Deserialize(a, SourceGenerationContext.Default.SingleArray);
                var vecB = JsonSerializer.Deserialize(b, SourceGenerationContext.Default.SingleArray);
                if (vecA == null || vecB == null)
                    return 0f;
                
                return _distanceFunction(vecA, vecB);
            });
        EnsureTable();

    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    void EnsureTable() {
        
        var createCommand = _connection.CreateCommand();
        string query = $"CREATE TABLE IF NOT EXISTS {_tableName} (id TEXT PRIMARY KEY, vector BLOB, document TEXT)";
        createCommand.CommandText = query;
        createCommand.ExecuteNonQuery();
        
    }

    private static string SerializeDocument(Document document)
    {
        return JsonSerializer.Serialize(document, SourceGenerationContext.Default.Document);
    }

    private static string SerializeVector(float[] vector)
    {
        return JsonSerializer.Serialize(vector, SourceGenerationContext.Default.SingleArray);
    }

    async Task InsertDocument(string id, float[] vector, Document document)
    {
        
        var insertCommand = _connection.CreateCommand();
        string query = $"INSERT INTO {_tableName} (id, vector, document) VALUES (@id, @vector, @document)";
        insertCommand.CommandText = query;
        insertCommand.Parameters.AddWithValue("@id", id);
        insertCommand.Parameters.AddWithValue("@vector", SerializeVector(vector));
        insertCommand.Parameters.AddWithValue("@document", SerializeDocument(document));
        await insertCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
        
    }

    async Task DeleteDocument(string id)
    {
        
        var deleteCommand = _connection.CreateCommand();
        string query = $"DELETE FROM {_tableName} WHERE id=@id";
        deleteCommand.CommandText = query;
        deleteCommand.Parameters.AddWithValue("@id", id);
        await deleteCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
        
    }

    async Task<List<(Document,float)>> SearchByVector(float[] vector, int k)
    {
        
        var searchCommand = _connection.CreateCommand();
        string query = $"SELECT id, vector, document, distance(vector, @vector) d FROM {_tableName} ORDER BY d LIMIT @k";
        searchCommand.CommandText = query;
        searchCommand.Parameters.AddWithValue("@vector", SerializeVector(vector));
        searchCommand.Parameters.AddWithValue("@k", k);
        var res = new List<(Document, float)>();
        var reader = await searchCommand.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            var id = reader.GetString(0);
            var vec = await reader.GetFieldValueAsync<string>(1).ConfigureAwait(false);
            var doc = await reader.GetFieldValueAsync<string>(2).ConfigureAwait(false);
            var docDeserialized = JsonSerializer.Deserialize(doc, SourceGenerationContext.Default.Document) ?? new Document("");
            var distance = reader.GetFloat(3);
            res.Add((docDeserialized, distance));
            
        }
        
        return res;
    }
    

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<Vector> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));
        
        foreach (var item in items)
        {
            if (item.Embedding is null)
            {
                throw new ArgumentException("Embedding is required", nameof(items));
            }
            
            await InsertDocument(item.Id, item.Embedding, new Document(item.Text, item.Metadata)).ConfigureAwait(false);
        }

        return items.Select(i => i.Id).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        ids = ids ?? throw new ArgumentNullException(nameof(ids));
        
        foreach (var id in ids)
            await DeleteDocument(id).ConfigureAwait(false);
        
        return true;
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        settings ??= new VectorSearchSettings();
        
        var documents = await SearchByVector(
            request.Embeddings.First(),
            settings.NumberOfResults).ConfigureAwait(false);
        
        return new VectorSearchResponse
        {
            Items = documents.Select(d => new Vector
            {
                Text = d.Item1.PageContent,
                Metadata = d.Item1.Metadata,
                Distance = d.Item2,
            }).ToArray(),
        };
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, Converters = [typeof(ObjectAsPrimitiveConverter)])]
[JsonSerializable(typeof(Document))]
[JsonSerializable(typeof(float[]))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;