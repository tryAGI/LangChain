// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AmazonV2EmbeddingSettings : BedrockEmbeddingSettings
{
    public new static AmazonV2EmbeddingSettings Default { get; } = new()
    {
        Dimensions = 1024,
        MaximumInputLength = 10_000,
        Normalize = true
    };

    public bool Normalize { get; set; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static AmazonV2EmbeddingSettings Calculate(
        EmbeddingSettings? requestSettings,
        EmbeddingSettings? modelSettings,
        EmbeddingSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as AmazonV2EmbeddingSettings;
        var modelSettingsCasted = modelSettings as AmazonV2EmbeddingSettings;
        var providerSettingsCasted = providerSettings as AmazonV2EmbeddingSettings;

        return new AmazonV2EmbeddingSettings
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

            Normalize =
                requestSettingsCasted?.Normalize ??
                modelSettingsCasted?.Normalize ??
                providerSettingsCasted?.Normalize ??
                Default.Normalize,
        };
    }
}