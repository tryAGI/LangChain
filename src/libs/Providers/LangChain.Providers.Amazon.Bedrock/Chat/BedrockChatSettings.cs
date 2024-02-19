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
        };
    }
}