using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using LangChain.Databases.JsonConverters;
using LangChain.DocumentLoaders;
using Microsoft.Data.Sqlite;

namespace LangChain.Databases.Sqlite;

/// <summary>
/// 
/// </summary>
public sealed class SqLiteVectorCollection : VectorCollection, IVectorCollection
{
    private readonly SqliteConnection _connection;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public SqLiteVectorCollection(
        SqliteConnection connection,
        string name = DefaultName,
        string? id = null) : base(name, id)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    private static string SerializeDocument(Document document)
    {
        return JsonSerializer.Serialize(document, SourceGenerationContext.Default.Document);
    }

    private static string SerializeVector(float[] vector)
    {
        return JsonSerializer.Serialize(vector, SourceGenerationContext.Default.SingleArray);
    }

    private async Task InsertDocument(string id, float[] vector, Document document)
    {
        var insertCommand = _connection.CreateCommand();
        string query = $"INSERT INTO {Name} (id, vector, document) VALUES (@id, @vector, @document)";
        insertCommand.CommandText = query;
        insertCommand.Parameters.AddWithValue("@id", id);
        insertCommand.Parameters.AddWithValue("@vector", SerializeVector(vector));
        insertCommand.Parameters.AddWithValue("@document", SerializeDocument(document));
        await insertCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

    }

    private async Task DeleteDocument(string id)
    {
        var deleteCommand = _connection.CreateCommand();
        string query = $"DELETE FROM {Name} WHERE id=@id";
        deleteCommand.CommandText = query;
        deleteCommand.Parameters.AddWithValue("@id", id);
        await deleteCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    private async Task<List<(Document, float)>> SearchByVector(float[] vector, int k)
    {
        var searchCommand = _connection.CreateCommand();
        string query = $"SELECT id, vector, document, distance(vector, @vector) d FROM {Name} ORDER BY d LIMIT @k";
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

    /// <inheritdoc />
    public async Task<Vector?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var command = _connection.CreateCommand();
        var query = $"SELECT vector, document FROM {Name} WHERE id=@id";
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        var vec = await reader.GetFieldValueAsync<string>(0, cancellationToken).ConfigureAwait(false);
        var doc = await reader.GetFieldValueAsync<string>(1, cancellationToken).ConfigureAwait(false);
        var docDeserialized = JsonSerializer.Deserialize(doc, SourceGenerationContext.Default.Document) ?? new Document("");

        return new Vector
        {
            Id = id,
            Text = docDeserialized.PageContent,
            Metadata = docDeserialized.Metadata,
            Embedding = JsonSerializer.Deserialize(vec, SourceGenerationContext.Default.SingleArray),
        };
    }

    /// <inheritdoc />
    public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        var command = _connection.CreateCommand();
        var query = $"SELECT COUNT(*) FROM {Name}";
        command.CommandText = query;
        var count = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

        return count == null || Convert.ToInt32(count, CultureInfo.InvariantCulture) == 0;
    }

    /// <inheritdoc />
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
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(float))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;