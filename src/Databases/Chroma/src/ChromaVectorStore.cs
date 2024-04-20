using System.Text.Json;
using System.Text.Json.Serialization;
using LangChain.Databases.JsonConverters;
using LangChain.Sources;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.Memory.Chroma;
using Microsoft.SemanticKernel.Connectors.Memory.Chroma.Http.ApiSchema;
using Microsoft.SemanticKernel.Memory;

namespace LangChain.Databases.Chroma;

/// <summary>
/// ChromaDB vector store.
/// According: https://api.python.langchain.com/en/latest/_modules/langchain/vectorstores/chroma.html
/// </summary>
public class ChromaVectorStore : IVectorDatabase
{
    private const string DefaultCollectionName = "langchain";

    // TODO: SemanticKernel impl doesn't support collection metadata. Need changes when moved to another impl
    //private Dictionary<string, string> CollectionMetadata { get; } = [];

    private readonly ChromaMemoryStore _store;

    private readonly ChromaClient _client;
    private readonly string _collectionName;

    public ChromaVectorStore(
        HttpClient httpClient,
        string endpoint,
        string collectionName = DefaultCollectionName)
    {
        _client = new ChromaClient(httpClient, endpoint);

        _collectionName = collectionName;

        _store = new ChromaMemoryStore(_client);
    }

    /// <summary>
    /// Get collection
    /// </summary>
    [CLSCompliant(false)]
    public async Task<ChromaCollectionModel?> GetCollectionAsync()
    {
        return await _client.GetCollectionAsync(_collectionName).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete collection
    /// </summary>
    public async Task DeleteCollectionAsync()
    {
        await _store.DeleteCollectionAsync(_collectionName).ConfigureAwait(false);
    }

    /// <summary>
    /// Get collection
    /// </summary>
    public async Task<Document?> GetDocumentByIdAsync(string id)
    {
        var record = await _store.GetAsync(_collectionName, id, withEmbedding: true).ConfigureAwait(false);

        if (record == null)
        {
            return null;
        }

        var text = record.Metadata.Text;
        var metadata = DeserializeMetadata(record.Metadata);

        return new Document(text, metadata);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        await _store.RemoveBatchAsync(_collectionName, ids, cancellationToken).ConfigureAwait(false);

        return true;
    }

    private static float SelectRelevanceScoreFn(float distance)
    {
        //const string distanceKey = "hnsw:space";

        var distanceType = "l2";
        //if (CollectionMetadata.TryGetValue(distanceKey, out var value))
        //{
        //    distanceType = value;
        //}

        return distanceType switch
        {
            "cosine" => RelevanceScoreFunctions.Cosine(distance),
            "l2" => RelevanceScoreFunctions.Euclidean(distance),
            "ip" => RelevanceScoreFunctions.MaxInnerProduct(distance),

            _ => throw new ArgumentException(
                $@"No supported normalization function for distance metric of type: {distanceType}.
                Consider providing relevance_score_fn to Chroma constructor.")
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<VectorSearchItem> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));
        
        var records = new MemoryRecord[items.Count];
        for (var index = 0; index < items.Count; index++)
        {
            var item = items.ElementAt(index);
            
            // TODO: check: description, externalSourceName, key
            records[index] = new MemoryRecord
            (
                new MemoryRecordMetadata
                (
                    isReference: false,
                    id: item.Id,
                    text: item.Text,
                    description: string.Empty,
                    externalSourceName: string.Empty,
                    additionalMetadata: SerializeMetadata(item.Metadata ?? new Dictionary<string, object>())
                ),
                new Embedding<float>(item.Embedding ?? []),
                key: null
            );
        }

        var collections =  _client.ListCollectionsAsync(cancellationToken).ConfigureAwait(false);
        var isCollectionExists = false;
        await foreach (var collectionName in collections)
        {
            if (collectionName == _collectionName)
            {
                isCollectionExists = true;
                break;
            }
        }
        if (!isCollectionExists)
        {
            await _client.CreateCollectionAsync(_collectionName, cancellationToken).ConfigureAwait(false);
        }
        
        var resultIds = new List<string>(items.Count);
        var resultIdsIterator = _store.UpsertBatchAsync(_collectionName, records, cancellationToken);
        await foreach (var item in resultIdsIterator.ConfigureAwait(false))
        {
            resultIds.Add(item);
        }

        return resultIds;
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        settings ??= new VectorSearchSettings();
        
        settings.RelevanceScoreFunc ??= SelectRelevanceScoreFn;
        
        var matches = await _store
            .GetNearestMatchesAsync(
                collectionName: _collectionName,
                embedding: new Embedding<float>(request.Embeddings.First()),
                limit: settings.NumberOfResults,
                cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new VectorSearchResponse
        {
            Items = matches
                .Select(record =>
                {
                    var text = record.Item1.Metadata.Text;
                    var metadata = DeserializeMetadata(record.Item1.Metadata);

                    return new VectorSearchItem
                    {
                        Id = record.Item1.Metadata.Id,
                        Text = text,
                        Metadata = metadata,
                        Embedding = record.Item1.Embedding.Vector.ToArray(),
                        Distance = (float)record.Item2
                    };
                })
                .ToArray(),
        };
    }

    private static string SerializeMetadata(IReadOnlyDictionary<string, object> metadata)
    {
        return JsonSerializer.Serialize(metadata, SourceGenerationContext.Default.IReadOnlyDictionaryStringObject);
    }

    private static IReadOnlyDictionary<string, object> DeserializeMetadata(MemoryRecordMetadata metadata)
    {
        return JsonSerializer.Deserialize(metadata.AdditionalMetadata, SourceGenerationContext.Default.IReadOnlyDictionaryStringObject)
               ?? new Dictionary<string, object>();
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, Converters = [typeof(ObjectAsPrimitiveConverter)])]
[JsonSerializable(typeof(IReadOnlyDictionary<string, object>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;