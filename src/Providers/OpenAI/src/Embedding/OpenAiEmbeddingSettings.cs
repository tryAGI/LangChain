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
    /// 
    /// </summary>
    public string? User { get; init; }

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
        };
    }
}