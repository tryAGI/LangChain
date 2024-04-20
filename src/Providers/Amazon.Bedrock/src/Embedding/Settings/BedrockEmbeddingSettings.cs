// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class BedrockEmbeddingSettings : EmbeddingSettings
{
    public new static BedrockEmbeddingSettings Default { get; } = new()
    {
        Dimensions = 1536,
        MaximumInputLength = 10_000
    };

    /// <summary>
    /// Dimensions of embeddings
    /// </summary>
    public int? Dimensions { get; init; }

    /// <summary>
    /// Maximum Input Length of text Prompt
    /// </summary>
    public int? MaximumInputLength { get; init; }


    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static BedrockEmbeddingSettings Calculate(
        EmbeddingSettings? requestSettings,
        EmbeddingSettings? modelSettings,
        EmbeddingSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as BedrockEmbeddingSettings;
        var modelSettingsCasted = modelSettings as BedrockEmbeddingSettings;
        var providerSettingsCasted = providerSettings as BedrockEmbeddingSettings;

        return new BedrockEmbeddingSettings
        {
            Dimensions =
                requestSettingsCasted?.Dimensions ??
                modelSettingsCasted?.Dimensions ??
                providerSettingsCasted?.Dimensions ??
                Default.Dimensions ??
                throw new InvalidOperationException("Default Dimensions is not set."),

            MaximumInputLength =
                requestSettingsCasted?.MaximumInputLength ??
                modelSettingsCasted?.MaximumInputLength ??
                providerSettingsCasted?.MaximumInputLength ??
                Default.MaximumInputLength ??
                throw new InvalidOperationException("Default MaximumInputLength is not set."),
        };
    }
}