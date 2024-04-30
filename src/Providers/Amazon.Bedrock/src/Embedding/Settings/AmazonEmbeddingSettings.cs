// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AmazonEmbeddingSettings : BedrockEmbeddingSettings
{
    public new static AmazonEmbeddingSettings Default { get; } = new()
    {
        Dimensions = 1536,
        MaximumInputLength = 10_000
    };

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static AmazonEmbeddingSettings Calculate(
        EmbeddingSettings? requestSettings,
        EmbeddingSettings? modelSettings,
        EmbeddingSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as AmazonEmbeddingSettings;
        var modelSettingsCasted = modelSettings as AmazonEmbeddingSettings;
        var providerSettingsCasted = providerSettings as AmazonEmbeddingSettings;

        return new AmazonEmbeddingSettings
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