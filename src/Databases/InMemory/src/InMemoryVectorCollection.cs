using System.Collections.Concurrent;

namespace LangChain.Databases.InMemory;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="name"></param>
/// <param name="distanceMetrics"></param>
public class InMemoryVectorCollection(
    string name = VectorCollection.DefaultName,
    EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean)
    : VectorCollection(name), IVectorCollection
{
    private readonly Func<float[], float[], float> _distanceFunction =
        distanceMetrics == EDistanceMetrics.Euclidean
            ? DistanceFunctions.Euclidean
            : DistanceFunctions.Manhattan;

    private readonly ConcurrentDictionary<string, Vector> _vectors = [];

    /// <inheritdoc />
    public Task<IReadOnlyCollection<string>> AddAsync(
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

            _vectors.TryAdd(item.Id, item);
        }

        return Task.FromResult<IReadOnlyCollection<string>>(items.Select(i => i.Id).ToArray());
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        ids = ids ?? throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
        {
            _vectors.TryRemove(id, out _);
        }

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        settings ??= new VectorSearchSettings();

        return Task.FromResult(new VectorSearchResponse
        {
            Items = _vectors
                .Select(d => new Vector
                {
                    Text = d.Value.Text,
                    Metadata = d.Value.Metadata,
                    Distance = _distanceFunction(d.Value.Embedding!, request.Embeddings.First()),
                })
                .OrderBy(d => d.Distance)
                .Take(settings.NumberOfResults)
                .ToArray(),
        });
    }

    /// <inheritdoc />
    public Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_vectors.IsEmpty);
    }

    /// <inheritdoc />
    public Task<Vector?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_vectors.GetValueOrDefault(id));
    }
}