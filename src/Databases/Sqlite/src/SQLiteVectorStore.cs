﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LangChain.Sources;
using LangChain.Providers;
using LangChain.VectorStores;
using Microsoft.Data.Sqlite;

namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
[RequiresDynamicCode("Requires dynamic code.")]
[RequiresUnreferencedCode("Requires unreferenced code.")]
public sealed class SQLiteVectorStore : VectorStore, IDisposable
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
    /// <param name="embeddings"></param>
    /// <param name="distanceMetrics"></param>
    public SQLiteVectorStore(
        string filename,
        string tableName,
        IEmbeddingModel embeddings,
        EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean)
        : base(embeddings)
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
                var vecA = JsonSerializer.Deserialize<float[]>(a);
                var vecB = JsonSerializer.Deserialize<float[]>(b);
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

    static string SerializeDocument(Document document)
    {
        return JsonSerializer.Serialize(document);
    }

    static string SerializeVector(float[] vector)
    {
        return JsonSerializer.Serialize(vector);
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
            var docDeserialized = JsonSerializer.Deserialize<Document>(doc) ?? new Document("");
            var distance = reader.GetFloat(3);
            res.Add((docDeserialized, distance));
            
        }
        
        return res;
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<string>> AddDocumentsAsync(
        IEnumerable<Document> documents,
        CancellationToken cancellationToken = default)
    {

        var docs = documents.ToArray();

        float[][] embeddings = await EmbeddingModel.CreateEmbeddingsAsync(
            docs
                .Select(x => x.PageContent)
                .ToArray(),
            cancellationToken: cancellationToken).ConfigureAwait(false);
        List<string> ids = new List<string>();
        for (int i = 0; i < docs.Length; i++)
        {
            var id = Guid.NewGuid().ToString();
            ids.Add(id);
            await InsertDocument(id, embeddings[i],  docs[i]).ConfigureAwait(false);
        }

        return ids;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="metadatas"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<string>> AddTextsAsync(IEnumerable<string> texts, IEnumerable<Dictionary<string, object>>? metadatas = null, CancellationToken cancellationToken = default)
    {
        if (metadatas != null)
        {
            var docs = texts.Zip(metadatas, (d, m) => new Document(d, m)).ToArray();
            return await AddDocumentsAsync(docs, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var docs = texts.Select(d => new Document(d)).ToArray();
            return await AddDocumentsAsync(docs, cancellationToken).ConfigureAwait(false);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        ids = ids ?? throw new ArgumentNullException(nameof(ids));
        
        foreach (var id in ids)
            await DeleteDocument(id).ConfigureAwait(false);
        
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="k"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<Document>> SimilaritySearchAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        float[] embedding = await EmbeddingModel.CreateEmbeddingsAsync(
            query,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await SimilaritySearchByVectorAsync(
            embedding,
            k,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="embedding"></param>
    /// <param name="k"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(IEnumerable<float> embedding, int k = 4, CancellationToken cancellationToken = default)
    {

        var arr = embedding.ToArray();
        var documents = await SearchByVector(arr, k).ConfigureAwait(false);
        return documents.Select(x=>x.Item1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="k"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(string query,
        int k = 4, CancellationToken cancellationToken = default)
    {
        float[] embedding = await EmbeddingModel.CreateEmbeddingsAsync(
            query,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        var documents = await SearchByVector(
            embedding,
            k).ConfigureAwait(false);
        
        return documents;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="embedding"></param>
    /// <param name="k"></param>
    /// <param name="fetchK"></param>
    /// <param name="lambdaMult"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(IEnumerable<float> embedding, int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="k"></param>
    /// <param name="fetchK"></param>
    /// <param name="lambdaMult"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(string query, int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override Func<float, float> SelectRelevanceScoreFn()
    {
        throw new NotImplementedException();
    }
}