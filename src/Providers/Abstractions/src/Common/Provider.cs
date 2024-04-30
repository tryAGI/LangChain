// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <inheritdoc cref="IProvider" />
public abstract class Provider(string id) : Model(id), IProvider
{
    /// <inheritdoc />
    public ChatSettings? ChatSettings { get; init; }

    /// <inheritdoc />
    public EmbeddingSettings? EmbeddingSettings { get; init; }

    /// <inheritdoc />
    public TextToImageSettings? TextToImageSettings { get; init; }

    /// <inheritdoc />
    public ModerationSettings? ModerationSettings { get; init; }

    /// <inheritdoc />
    public SpeechToTextSettings? SpeechToTextSettings { get; init; }

    /// <inheritdoc />
    public TextToSpeechSettings? TextToSpeechSettings { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public ImageToTextSettings? ImageToTextSettings { get; init; }
}