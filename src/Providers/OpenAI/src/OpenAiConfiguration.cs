namespace LangChain.Providers.OpenAI;

/// <summary>
/// 
/// </summary>
public class OpenAiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public const string SectionName = "OpenAI";

    /// <summary>
    /// 
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Endpoint { get; set; }
    
    public OpenAiChatSettings ChatSettings { get; set; }
    
    public EmbeddingSettings EmbeddingSettings { get; init; } = new();
    
    public TextToImageSettings TextToImageSettings { get; init; } = new();
    
    public ModerationSettings ModerationSettings { get; init; } = new();
    
    public SpeechToTextSettings SpeechToTextSettings { get; init; } = new();
    
    public TextToSpeechSettings TextToSpeechSettings { get; init; } = new();

    public ImageToTextSettings ImageToTextSettings { get; init; } = new();

    public OpenAiConfiguration(
        string? apiKey = null,
        string? endpoint = null,
        OpenAiChatSettings? chatSettings = null)
    {
        ApiKey = apiKey;
        Endpoint = endpoint;
        ChatSettings = chatSettings ?? new();
    }
}