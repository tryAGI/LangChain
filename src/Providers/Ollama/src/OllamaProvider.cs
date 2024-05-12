using Ollama;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
/// <param name="url"></param>
/// <param name="options"></param>
public class OllamaProvider(
    string url = "http://localhost:11434",
    RequestOptions? options = null)
    : Provider(id: "ollama")
{
    /// <summary>
    /// 
    /// </summary>
    public OllamaApiClient Api { get; } = new(new HttpClient
    {
        Timeout = TimeSpan.FromHours(1),
        BaseAddress = new Uri(url),
    });

    /// <summary>
    /// 
    /// </summary>
    public RequestOptions Options { get; } = options ?? new RequestOptions();
}