// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AnthropicChatSettings : BedrockChatSettings
{
    public new static AnthropicChatSettings Default { get; } = new()
    {
        StopSequences = ChatSettings.Default.StopSequences,
        User = ChatSettings.Default.User,
        UseStreaming = false,
        Temperature = 0.7,
        MaxTokens = 100_000,
        TopP = 0.9,
        TopK = 0.0
    };

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static AnthropicChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as AnthropicChatSettings;
        var modelSettingsCasted = modelSettings as AnthropicChatSettings;
        var providerSettingsCasted = providerSettings as AnthropicChatSettings;

        return new AnthropicChatSettings
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
            UseStreaming =
                requestSettings?.UseStreaming ??
                modelSettings?.UseStreaming ??
                providerSettings?.UseStreaming ??
                Default.UseStreaming ??
                throw new InvalidOperationException("Default UseStreaming is not set."),
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