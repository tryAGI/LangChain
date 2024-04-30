namespace LangChain.Databases;

/// <summary>
/// Vector search item.
/// </summary>
public class Vector
{
    /// <summary>
    /// 
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyDictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public float[]? Embedding { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public float Distance { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public float RelevanceScore { get; set; }
}