namespace LangChain.Providers.Google;

public class GoogleGeminiChatSettings : ChatSettings
{
    public new static GoogleGeminiChatSettings Default { get; } = new()
    {
        StopSequences = ChatSettings.Default.StopSequences,
        User = ChatSettings.Default.User,
        UseStreaming = ChatSettings.Default.UseStreaming,
        Temperature = 1.0
    };

    /// <summary>
    ///     Sampling temperature
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    ///     Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static GoogleGeminiChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as GoogleGeminiChatSettings;
        var modelSettingsCasted = modelSettings as GoogleGeminiChatSettings;
        var providerSettingsCasted = providerSettings as GoogleGeminiChatSettings;

        return new GoogleGeminiChatSettings
        {
            StopSequences =
                requestSettings?.StopSequences ??
                modelSettings?.StopSequences ??
                providerSettings?.StopSequences ??
                Default.StopSequences ??
                throw new InvalidOperationException("Default StopSequences is not set."),
            User =
                requestSettings?.User ??
                modelSettings?.User ??
                providerSettings?.User ??
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
                throw new InvalidOperationException("Default Temperature is not set.")
        };
    }
}