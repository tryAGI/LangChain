// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.SageMaker;

public class SageMakerChatSettings : ChatSettings
{
    public new static SageMakerChatSettings Default { get; } = new()
    {
        StopSequences = ChatSettings.Default.StopSequences,
        User = ChatSettings.Default.User,
        MaxNewTokens = 256,
    };

    public int? MaxNewTokens { get; set; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static SageMakerChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as SageMakerChatSettings;
        var modelSettingsCasted = modelSettings as SageMakerChatSettings;
        var providerSettingsCasted = providerSettings as SageMakerChatSettings;
        
        return new SageMakerChatSettings
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
            MaxNewTokens = 
                requestSettingsCasted?.MaxNewTokens ??
                modelSettingsCasted?.MaxNewTokens ??
                providerSettingsCasted?.MaxNewTokens ??
                Default.MaxNewTokens ??
                throw new InvalidOperationException("Default MaxNewTokens is not set."),
        };
    }
}