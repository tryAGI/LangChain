namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
/// <param name="url"></param>
/// <param name="options"></param>
public class OllamaProvider(
    string url = "http://localhost:11434",
    OllamaOptions? options = null)
    : Provider(id: "ollama")
{
    /// <summary>
    /// 
    /// </summary>
    public OllamaApiClient Api { get; } = new(url);

    /// <summary>
    /// 
    /// </summary>
    public OllamaOptions Options { get; } = options ?? new OllamaOptions();
}