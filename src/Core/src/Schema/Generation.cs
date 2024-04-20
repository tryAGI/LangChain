namespace LangChain.Schema;

/// <summary>
/// 
/// </summary>
public class Generation
{
    /// <summary>
    /// Generated text output
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Raw generation info response from the provider.
    /// May include things like reason for finishing (e.g. in OpenAi)
    /// </summary>
    public Dictionary<string, object> GenerationInfo { get; set; } = new Dictionary<string, object>();
}