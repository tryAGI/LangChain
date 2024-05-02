namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public static class DataSourceExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public static IReadOnlyDictionary<string, object> ToMetadata(this DataSource dataSource)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(paramName: nameof(dataSource));
        
        return new Dictionary<string, object>
        {
            ["encoding"] = dataSource.Encoding.EncodingName,
            ["path"] = dataSource.Value ?? string.Empty,
            ["type"] = dataSource.Type.ToString(),
        };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public static IReadOnlyDictionary<string, object>? CollectMetadata(
        this DocumentLoaderSettings? settings,
        DataSource dataSource)
    {
        return settings?.ShouldCollectMetadata == true
            ? dataSource.ToMetadata()
            : null;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="additionalMetadata"></param>
    /// <returns></returns>
    public static IReadOnlyDictionary<string, object>? With(
        this IReadOnlyDictionary<string, object>? metadata,
        IReadOnlyDictionary<string, object> additionalMetadata)
    {
        return metadata?
            .Concat(additionalMetadata)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}