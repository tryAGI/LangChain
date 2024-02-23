// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public class OpenAiChatSettings : ChatSettings
{
    public new static OpenAiChatSettings Default { get; } = new()
    {
        StopSequences = ChatSettings.Default.StopSequences,
        User = ChatSettings.Default.User,
        Temperature = 1.0,
    };

    /// <summary>
    /// Sampling temperature
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static OpenAiChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as OpenAiChatSettings;
        var modelSettingsCasted = modelSettings as OpenAiChatSettings;
        var providerSettingsCasted = providerSettings as OpenAiChatSettings;
        
        return new OpenAiChatSettings
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
        };
    }
}