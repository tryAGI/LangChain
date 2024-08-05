using Elastic.Clients.Elasticsearch;

namespace LangChain.Databases.Elasticsearch;

/// <summary>
/// Elasticsearch vector collection.
/// </summary>
public class ElasticsearchVectorCollection(
    ElasticsearchClient client,
    string name = VectorCollection.DefaultName,
    string? id = null)
    : VectorCollection(name, id), IVectorCollection
{
    /// <inheritdoc />
    public async Task<Vector?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        // var record = await client.GetAsync(Name, new Id(id), cancellationToken: cancellationToken).ConfigureAwait(false);
        // if (record == null)
        // {
        //     return null;
        // }
        //
        // return new Vector
        // {
        //     Text = string.Empty,
        //     Metadata = new Dictionary<string, object>(),
        // };
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        ids = ids ?? throw new ArgumentNullException(nameof(ids));

        throw new NotImplementedException();
        // foreach (var id in ids)
        // {
        //     await client.DeleteAsync(Name, new Id(id), cancellationToken).ConfigureAwait(false);
        // }
        //
        // return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<Vector> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));

        throw new NotImplementedException();
        //return Task.FromResult<IReadOnlyCollection<string>>([]);
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        settings ??= new VectorSearchSettings();

        throw new NotImplementedException();
        // var response = await client.SearchAsync<MyDoc>(s => s
        //     .Index("my_index")
        //     .From(0)
        //     .Size(10)
        //     .Query(q => q
        //         .Knn()
        //         .Term(t => t.User, "flobernd")
        //     )
        // );
        //
        // if (response.IsValidResponse)
        // {
        //     var doc = response.Documents.FirstOrDefault();
        // }
        //
        // return Task.FromResult(new VectorSearchResponse
        // {
        //     Items = Array.Empty<string>()
        //         .Select(record =>
        //         {
        //             return new Vector
        //             {
        //                 Id = string.Empty,
        //                 Text = string.Empty,
        //                 Metadata = new Dictionary<string, object>(),
        //                 Embedding = [],
        //                 Distance = 0.0F,
        //             };
        //         })
        //         .ToArray(),
        // });
    }

    /// <inheritdoc />
    public Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}