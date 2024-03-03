// ReSharper disable once CheckNamespace

namespace LangChain.Providers.Amazon.SageMaker;

public class SageMakerSettings : ChatSettings
{
    public new static SageMakerSettings Default { get; } = new()
    {
        User = ChatSettings.Default.User,
        ContentType = "application/json",
        Accept = "*/*",
    };

    /// <summary>
    /// The Accept request HTTP header
    /// </summary>
    public string? Accept { get; set; }

    /// <summary>
    /// The Content-Type representation header
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// SageMaker input parameters
    /// </summary>
    public Dictionary<string, object>? InputParamers { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Func<HttpResponseMessage, object>? TransformOutput { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static SageMakerSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as SageMakerSettings;
        var modelSettingsCasted = modelSettings as SageMakerSettings;
        var providerSettingsCasted = providerSettings as SageMakerSettings;

        return new SageMakerSettings
        {
            User =
                requestSettingsCasted?.User ??
                modelSettingsCasted?.User ??
                providerSettingsCasted?.User ??
                Default.User ??
                throw new InvalidOperationException("Default User is not set."),
            Accept =
                requestSettingsCasted?.Accept ??
                modelSettingsCasted?.Accept ??
                providerSettingsCasted?.Accept ??
                Default.Accept ??
                throw new InvalidOperationException("Default Accept is not set."),
            ContentType =
                requestSettingsCasted?.ContentType ??
                modelSettingsCasted?.ContentType ??
                providerSettingsCasted?.ContentType ??
                Default.ContentType ??
                throw new InvalidOperationException("Default ContentType is not set."),
            InputParamers =
                requestSettingsCasted?.InputParamers ??
                modelSettingsCasted?.InputParamers ??
                providerSettingsCasted?.InputParamers ??
                Default.InputParamers ??
                throw new InvalidOperationException("Default InputParamers is not set."),
            TransformOutput = requestSettingsCasted?.TransformOutput ??
                              modelSettingsCasted?.TransformOutput ??
                              providerSettingsCasted?.TransformOutput
        };
    }
}