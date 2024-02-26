// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class BedrockChatSettings : ChatSettings
{
    public new static BedrockChatSettings Default { get; } = new()
    {
        StopSequences = ChatSettings.Default.StopSequences,
        User = ChatSettings.Default.User,
        Temperature = 0.7,
        MaxTokens = 4096,
        TopP = 0.9,
        TopK = 0.0
    };

    /// <summary>
    /// Sampling temperature
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// The cumulative probability cutoff for token selection.
    /// Lower values mean sampling from a smaller, more top-weighted nucleus
    /// </summary>
    public double? TopP { get; init; }

    /// <summary>
    /// Sample from the k most likely next tokens at each step.
    /// Lower k focuses on higher probability tokens.
    /// </summary>
    public double? TopK { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static BedrockChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as BedrockChatSettings;
        var modelSettingsCasted = modelSettings as BedrockChatSettings;
        var providerSettingsCasted = providerSettings as BedrockChatSettings;

        return new BedrockChatSettings
        {
            StopSequences =
                requestSettingsCasted?.StopSequences ??
                modelSettingsCasted?.StopSequences ??
                providerSettingsCasted?.StopSequences ??
                Default.StopSequences ??
                throw new InvalidOperationException("Default StopSequences is not set."),
            User =
                requestSettingsCasted?.User ??
                modelSettingsCasted?.User ??
                providerSettingsCasted?.User ??
                Default.User ??
                throw new InvalidOperationException("Default User is not set."),
            Temperature =
                requestSettingsCasted?.Temperature ??
                modelSettingsCasted?.Temperature ??
                providerSettingsCasted?.Temperature ??
                Default.Temperature ??
                throw new InvalidOperationException("Default Temperature is not set."),
            MaxTokens =
                requestSettingsCasted?.MaxTokens ??
                modelSettingsCasted?.MaxTokens ??
                providerSettingsCasted?.MaxTokens ??
                Default.MaxTokens ??
                throw new InvalidOperationException("Default MaxTokens is not set."),
            TopP =
                requestSettingsCasted?.TopP ??
                modelSettingsCasted?.TopP ??
                providerSettingsCasted?.TopP ??
                Default.TopP ??
                throw new InvalidOperationException("Default TopP is not set."),
            TopK =
                requestSettingsCasted?.TopK ??
                modelSettingsCasted?.TopK ??
                providerSettingsCasted?.TopK ??
                Default.TopK ??
                throw new InvalidOperationException("Default TopK is not set."),
        };
    }
}