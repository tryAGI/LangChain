using Microsoft.SemanticKernel.Memory;

namespace LangChain.Databases.SemanticKernel;

public class SemanticKernelMemoryStoreCollection(IMemoryStore store,
    string name = VectorCollection.DefaultName,
    string? id = null)
    : VectorCollection(name, id), IVectorCollection
{
    public async Task<IReadOnlyCollection<string>> AddAsync(IReadOnlyCollection<Vector> items, CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));

        List<string> list = [];
        foreach (var item in items)
        {
            string? metadata = null;
            //TODO: review way to map metadata
            if (item.Metadata != null)
                metadata = string.Join("#", item.Metadata.Select(kv => kv.Key + "&" + kv.Value));

            var record = new MemoryRecord
            (
                new MemoryRecordMetadata
                (
                    isReference: false,
                    id: item.Id,
                    text: item.Text,
                    description: string.Empty,
                    externalSourceName: item.Text,
                    additionalMetadata: metadata ?? string.Empty
                ),
                item.Embedding,
                null,
                null
            );

            var insert = await store.UpsertAsync(Name, record, cancellationToken).ConfigureAwait(false);
            list.Add(insert);
        }
        return list;
    }

    public async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        await store.RemoveBatchAsync(Name, ids, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<Vector?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var record = await store.GetAsync(Name, id, cancellationToken: cancellationToken).ConfigureAwait(false);

        Dictionary<string, object>? metadata = null;
        if (record?.Metadata?.AdditionalMetadata != null)
            metadata = record.Metadata.AdditionalMetadata
                .Split('#')
                .Select(part => part.Split('&'))
                .ToDictionary(split => split[0], split => (object)split[1]);

        return record != null ? new Vector { Id = id, Text = record.Metadata.ExternalSourceName, Metadata = metadata } : null;
    }

    public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        var collections = store.GetCollectionsAsync(cancellationToken);
        return !(await collections.CountAsync(cancellationToken).ConfigureAwait(false) > 0);
    }

    public async Task<VectorSearchResponse> SearchAsync(VectorSearchRequest request, VectorSearchSettings? settings = null, CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        settings ??= new VectorSearchSettings();
        var results = await store.GetNearestMatchesAsync(Name, request.Embeddings.First(), limit: settings.NumberOfResults, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return new VectorSearchResponse { Items = results.Select(x => new Vector { Text = x.Item1.Metadata.ExternalSourceName }).ToList() };
    }
}