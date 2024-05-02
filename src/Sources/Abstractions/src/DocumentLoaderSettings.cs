namespace LangChain.Sources;

/// <summary>
/// Document loader settings used to configure the behavior of the document loader.
/// </summary>
public class DocumentLoaderSettings
{
    /// <summary>
    /// Should the document loader collect metadata about the document. <br/>
    /// Default is true.
    /// </summary>
    public bool ShouldCollectMetadata { get; set; } = true;
}