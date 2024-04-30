namespace LangChain.Databases.OpenSearch;

/// <summary>
/// Represents a vector record stored in OpenSearch.
/// </summary>
public class VectorRecord
{
    /// <summary>
    /// Gets or sets the unique identifier of the vector record.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the text associated with the vector record.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the vector representation of the record.
    /// </summary>
    public required float[] Vector { get; set; }

    /// <summary>
    /// Gets or sets the metadata associated with the vector record.
    /// </summary>
    public IDictionary<string, object>? Metadata { get; set; }
}