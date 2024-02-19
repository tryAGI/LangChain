// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Defines a provider. Can provide large language models, embeddings, text to speech, speech to text, etc. <br/>
/// Should define authentication and total usage.
/// </summary>
public interface IProvider : IModel
{
    /// <summary>
    /// Defines the settings for the provider. <br/>
    /// These settings will be used as default settings for requests,
    /// but you can override them in the model or request. <br/>
    /// If not set, the model will use default settings for specific provider. <br/>
    /// </summary>
    public ChatSettings? ChatSettings { get; init; }
    
    /// <summary>
    /// Defines the settings for the provider. <br/>
    /// These settings will be used as default settings for requests,
    /// but you can override them in the model or request. <br/>
    /// If not set, the model will use default settings for specific provider. <br/>
    /// </summary>
    public EmbeddingSettings? EmbeddingSettings { get; init; }
    
    /// <summary>
    /// Defines the settings for the provider. <br/>
    /// These settings will be used as default settings for requests,
    /// but you can override them in the model or request. <br/>
    /// If not set, the model will use default settings for specific provider. <br/>
    /// </summary>
    public ImageGenerationSettings? ImageGenerationSettings { get; init; }
    
    /// <summary>
    /// Defines the settings for the provider. <br/>
    /// These settings will be used as default settings for requests,
    /// but you can override them in the model or request. <br/>
    /// If not set, the model will use default settings for specific provider. <br/>
    /// </summary>
    public ModerationSettings? ModerationSettings { get; init; }
    
    /// <summary>
    /// Defines the settings for the provider. <br/>
    /// These settings will be used as default settings for requests,
    /// but you can override them in the model or request. <br/>
    /// If not set, the model will use default settings for specific provider. <br/>
    /// </summary>
    public SpeechToTextSettings? SpeechToTextSettings { get; init; }
    
    /// <summary>
    /// Defines the settings for the provider. <br/>
    /// These settings will be used as default settings for requests,
    /// but you can override them in the model or request. <br/>
    /// If not set, the model will use default settings for specific provider. <br/>
    /// </summary>
    public TextToSpeechSettings? TextToSpeechSettings { get; init; }
}