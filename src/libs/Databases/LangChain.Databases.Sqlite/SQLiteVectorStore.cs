using System.Text.Json;
using LangChain.Abstractions.Embeddings.Base;
using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.TextSplitters;
using LangChain.VectorStores;
using Microsoft.Data.Sqlite;

namespace LangChain.Databases;

public class SQLiteVectorStore : VectorStore, IDisposable
{
    private readonly string _tableName;
    private readonly Func<float[], float[], float> _distanceFunction;
    private readonly SqliteConnection _connection;

    public static async Task<VectorStoreIndexWrapper> CreateIndexFromDocuments(
        IEmbeddings embeddings,
        IReadOnlyCollection<Document> documents, string filename="vectorstore.db",
        string tableName="vectors")
    {
        SQLiteVectorStore vectorStore = new SQLiteVectorStore(filename,tableName,embeddings);
        var textSplitter = new CharacterTextSplitter();
        VectorStoreIndexCreator indexCreator = new VectorStoreIndexCreator(vectorStore, textSplitter);
        var index = await indexCreator.FromDocumentsAsync(documents).ConfigureAwait(false);
        return index;
    }

    public SQLiteVectorStore(string filename, string tableName,IEmbeddings embeddings, EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean) : base(embeddings)
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
                return _distanceFunction(vecA, vecB);
            });
        EnsureTable();

    }

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

    string SerializeDocument(Document document)
    {
        return JsonSerializer.Serialize(document);
    }

    string SerializeVector(float[] vector)
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
        await insertCommand.ExecuteNonQueryAsync();
        
    }

    async Task DeleteDocument(string id)
    {
        
        var deleteCommand = _connection.CreateCommand();
        string query = $"DELETE FROM {_tableName} WHERE id=@id";
        deleteCommand.CommandText = query;
        deleteCommand.Parameters.AddWithValue("@id", id);
        await deleteCommand.ExecuteNonQueryAsync();
        
    }

    async Task<List<(Document,float)>> SearchByVector(float[] vector, int k)
    {
        
        var searchCommand = _connection.CreateCommand();
        string query = $"SELECT id, vector, document, distance(vector, @vector) d FROM {_tableName} ORDER BY d LIMIT @k";
        searchCommand.CommandText = query;
        searchCommand.Parameters.AddWithValue("@vector", SerializeVector(vector));
        searchCommand.Parameters.AddWithValue("@k", k);
        var res = new List<(Document, float)>();
        var reader = await searchCommand.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var id = reader.GetString(0);
            var vec = reader.GetFieldValue<string>(1);
            var doc = reader.GetFieldValue<string>(2);
            var docDeserialized = JsonSerializer.Deserialize<Document>(doc);
            var distance = reader.GetFloat(3);
            res.Add((docDeserialized, distance));
            
        }
        
        return res;
    }
    

    public override async Task<IEnumerable<string>> AddDocumentsAsync(
        IEnumerable<Document> documents,
        CancellationToken cancellationToken = default)
    {

        var docs = documents.ToArray();

        var embeddings = await Embeddings.EmbedDocumentsAsync(docs.Select(x => x.PageContent).ToArray(), cancellationToken).ConfigureAwait(false);
        List<string> ids = new List<string>();
        for (int i = 0; i < docs.Length; i++)
        {
            var id = Guid.NewGuid().ToString();
            ids.Add(id);
            await InsertDocument(id, embeddings[i],  docs[i]);
        }

        return ids;
    }

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

    public override async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
            await DeleteDocument(id);
        
        return true;
    }

    public override async Task<IEnumerable<Document>> SimilaritySearchAsync(string query, int k = 4, CancellationToken cancellationToken = default)
    {
        var embedding = await Embeddings.EmbedQueryAsync(query, cancellationToken).ConfigureAwait(false);
        return await SimilaritySearchByVectorAsync(embedding, k, cancellationToken).ConfigureAwait(false);
    }

    public override async Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(IEnumerable<float> embedding, int k = 4, CancellationToken cancellationToken = default)
    {

        var arr = embedding.ToArray();
        var documents = await SearchByVector(arr, k);
        return documents.Select(x=>x.Item1);
    }

    public override async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(string query,
        int k = 4, CancellationToken cancellationToken = default)
    {
        var embedding = await Embeddings.EmbedQueryAsync(query, cancellationToken).ConfigureAwait(false);
        var arr = embedding.ToArray();
        var documents = await SearchByVector(arr, k);
        return documents;
    }

    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(IEnumerable<float> embedding, int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(string query, int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override Func<float, float> SelectRelevanceScoreFn()
    {
        throw new NotImplementedException();
    }
}