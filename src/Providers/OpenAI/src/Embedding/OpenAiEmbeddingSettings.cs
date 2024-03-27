// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class OpenAiEmbeddingSettings : EmbeddingSettings
{
    /// <summary>
    /// 
    /// </summary>
    public new static OpenAiEmbeddingSettings Default { get; } = new()
    {
        User = null,
    };

    /// <summary>
    /// The user associated with this embedding request.
    /// </summary>
    public string? User { get; init; }

    /// <summary>
    /// The model to use for the embedding.
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Sampling temperature.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// The maximum number of tokens to generate in the completion.
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OpenAiEmbeddingSettings Calculate(
        EmbeddingSettings? requestSettings,
        EmbeddingSettings? modelSettings,
        EmbeddingSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as OpenAiEmbeddingSettings;
        var modelSettingsCasted = modelSettings as OpenAiEmbeddingSettings;
        var providerSettingsCasted = providerSettings as OpenAiEmbeddingSettings;

        return new OpenAiEmbeddingSettings
        {
            User =
                requestSettingsCasted?.User ??
                modelSettingsCasted?.User ??
                providerSettingsCasted?.User ??
                Default.User,
            Model =
                requestSettingsCasted?.Model ??
                modelSettingsCasted?.Model ??
                providerSettingsCasted?.Model ??
                Default.Model,
            Temperature =
                requestSettingsCasted?.Temperature ??
                modelSettingsCasted?.Temperature ??
                providerSettingsCasted?.Temperature ??
                Default.Temperature,
            MaxTokens =
                requestSettingsCasted?.MaxTokens ??
                modelSettingsCasted?.MaxTokens ??
                providerSettingsCasted?.MaxTokens ??
                Default.MaxTokens,
        };
    }
}