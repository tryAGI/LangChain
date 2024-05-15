using OpenSearch.Client;
using OpenSearch.Net;

namespace LangChain.Databases.OpenSearch;

/// <summary>
/// Represents a vector collection using OpenSearch.
/// </summary>
public class OpenSearchVectorCollection(
    OpenSearchClient client,
    string name,
    string id) : VectorCollection(name, id), IVectorCollection
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<Vector> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));

        var bulkDescriptor = new BulkDescriptor()
            .Refresh(Refresh.WaitFor);

        foreach (var item in items)
        {
            var content = item.Text.Trim();
            if (string.IsNullOrEmpty(content))
            {
                continue;
            }

            var vectorRecord = new VectorRecord
            {
                Id = item.Id,
                Text = content,
                Vector = item.Embedding ?? [],
                Metadata = item.Metadata?.ToDictionary(x => x.Key, x => x.Value),
            };

            bulkDescriptor.Index<VectorRecord>(
                indexDescriptor => indexDescriptor
                    .Document(vectorRecord)
                    .Index(Name)
            );
        }

        var response = await client.BulkAsync(bulkDescriptor, cancellationToken)
            .ConfigureAwait(false);
        if (response.Errors)
        {
            throw new InvalidOperationException($"Failed to add items to collection '{Name}'. DebugInformation: {response.DebugInformation}");
        }

        return response.Items
            .Select(i => i.Id)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<Vector?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync<VectorRecord>(
            request: new GetRequest(Name, id),
            ct: cancellationToken).ConfigureAwait(false);
        if (response.Source == null)
        {
            return null;
        }

        return new Vector
        {
            Id = response.Source.Id,
            Text = response.Source.Text ?? string.Empty,
            Metadata = response.Source.Metadata?
                .ToDictionary(x => x.Key, x => x.Value),
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        IList<bool> success = [];
        foreach (var request in ids.Select(id => new DeleteRequest(Name, id)))
        {
            var response = await client.DeleteAsync(request, cancellationToken).ConfigureAwait(false);
            success.Add(response.IsValid);
        }

        return success.All(x => x);
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        settings ??= new VectorSearchSettings();

        var response = await client.SearchAsync<VectorRecord>(s => s
            .Index(Name)
            .Query(q => q
                .Knn(knn => knn
                    .Field(f => f.Vector)
                    .Vector(request.Embeddings.First())
                    .K(settings.NumberOfResults)
                )
            ), cancellationToken).ConfigureAwait(false);
        if (!response.IsValid)
        {
            throw new InvalidOperationException($"Failed to search collection '{Name}'. DebugInformation: {response.DebugInformation}");
        }

        return new VectorSearchResponse
        {
            Items = response.Documents
                .Where(vectorRecord => !string.IsNullOrWhiteSpace(vectorRecord.Text))
                .Select(vectorRecord => new Vector
                {
                    Text = vectorRecord.Text ?? string.Empty,
                    Id = vectorRecord.Id,
                    Metadata = vectorRecord.Metadata?.ToDictionary(x => x.Key, x => x.Value),
                    Embedding = vectorRecord.Vector,
                })
                .ToArray()
        };
    }

    /// <inheritdoc />
    public Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}