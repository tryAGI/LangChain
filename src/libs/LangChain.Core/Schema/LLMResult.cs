namespace LangChain.Schema;

/// <summary>
/// 
/// </summary>
public class LlmResult
{
    /// <summary>
    /// List of the things generated. Each input could have multiple <see cref="Generation"/>, hence this is a list of lists.
    /// </summary>
    public IReadOnlyList<Generation[]> Generations { get; set; } = Array.Empty<Generation[]>();

    /// <summary>
    /// Dictionary of arbitrary LLM-provider specific output.
    /// </summary>
    public Dictionary<string, object> LlmOutput { get; set; } = new Dictionary<string, object>();
}