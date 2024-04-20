namespace LangChain.Databases.InMemory;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="distanceMetrics"></param>
public class InMemoryVectorStore(
    EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean)
    : IVectorDatabase
{
    private readonly Func<float[], float[], float> _distanceFunction =
        distanceMetrics == EDistanceMetrics.Euclidean
            ? DistanceFunctions.Euclidean
            : DistanceFunctions.Manhattan;

    private readonly Dictionary<string, VectorSearchItem> _storage = [];

    /// <inheritdoc />
    public Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<VectorSearchItem> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));
        
        foreach (var item in items)
        {
            if (item.Embedding is null)
            {
                throw new ArgumentException("Embedding is required", nameof(items));
            }
            
            _storage.Add(item.Id, item);
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
            _storage.Remove(id);
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
            Items = _storage
                .Select(d => new VectorSearchItem
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
}