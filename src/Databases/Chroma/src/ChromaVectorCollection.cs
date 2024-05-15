using System.Text.Json;
using System.Text.Json.Serialization;
using LangChain.Databases.JsonConverters;
using Microsoft.SemanticKernel.Connectors.Chroma;
using Microsoft.SemanticKernel.Memory;

namespace LangChain.Databases.Chroma;

/// <summary>
/// ChromaDB vector collection.
/// According: https://api.python.langchain.com/en/latest/_modules/langchain/vectorstores/chroma.html
/// </summary>
public class ChromaVectorCollection(
    ChromaMemoryStore store,
    string name = VectorCollection.DefaultName,
    string? id = null)
    : VectorCollection(name, id), IVectorCollection
{
    // TODO: SemanticKernel impl doesn't support collection metadata. Need changes when moved to another impl
    //private Dictionary<string, string> CollectionMetadata { get; } = [];

    /// <inheritdoc />
    public async Task<Vector?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var record = await store.GetAsync(Name, id, withEmbedding: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (record == null)
        {
            return null;
        }

        var text = record.Metadata.Text;
        var metadata = DeserializeMetadata(record.Metadata);

        return new Vector
        {
            Text = text,
            Metadata = metadata,
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        await store.RemoveBatchAsync(Name, ids, cancellationToken).ConfigureAwait(false);

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
        IReadOnlyCollection<Vector> items,
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
                new ReadOnlyMemory<float>(item.Embedding ?? []),
                key: null
            );
        }

        var resultIds = new List<string>(items.Count);
        var resultIdsIterator = store.UpsertBatchAsync(Name, records, cancellationToken);
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

        var matches = await store
            .GetNearestMatchesAsync(
                collectionName: Name,
                embedding: new System.ReadOnlyMemory<float>(request.Embeddings.First()),
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

                    return new Vector
                    {
                        Id = record.Item1.Metadata.Id,
                        Text = text,
                        Metadata = metadata,
                        Embedding = record.Item1.Embedding.ToArray(),
                        Distance = (float)record.Item2
                    };
                })
                .ToArray(),
        };
    }

    /// <inheritdoc />
    public Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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

[JsonSourceGenerationOptions(Converters = [typeof(ObjectAsPrimitiveConverter)])]
[JsonSerializable(typeof(IReadOnlyDictionary<string, object>))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(decimal))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;