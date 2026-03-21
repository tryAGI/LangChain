using Microsoft.Extensions.VectorData;

namespace LangChain.Schema;

/// <summary>
/// A built-in MEVA record type for LangChain's convenience layer (RAG helpers, chains).
/// </summary>
public class LangChainDocumentRecord
{
    /// <summary>
    /// Unique identifier for the record.
    /// </summary>
    [VectorStoreKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The text content of the document.
    /// </summary>
    [VectorStoreData]
    public string? Text { get; set; }

    /// <summary>
    /// JSON-serialized metadata dictionary.
    /// </summary>
    [VectorStoreData]
    public string? MetadataJson { get; set; }

    /// <summary>
    /// The embedding vector for this document.
    /// </summary>
    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
