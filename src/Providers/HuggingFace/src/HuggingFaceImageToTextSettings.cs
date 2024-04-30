// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for image to text request settings.
/// </summary>
public class HuggingFaceImageToTextSettings : ImageToTextSettings
{
    public new static HuggingFaceImageToTextSettings Default { get; } = new()
    {
        User = ImageToTextSettings.Default.User,
        Endpoint = "https://api-inference.huggingface.co/models/"
    };

    /// <summary>
    /// Endpoint url for api.
    /// </summary>
    public string? Endpoint { get; set; }


    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static HuggingFaceImageToTextSettings Calculate(
        ImageToTextSettings? requestSettings,
        ImageToTextSettings? modelSettings,
        ImageToTextSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as HuggingFaceImageToTextSettings;
        var modelSettingsCasted = modelSettings as HuggingFaceImageToTextSettings;
        var providerSettingsCasted = providerSettings as HuggingFaceImageToTextSettings;

        return new HuggingFaceImageToTextSettings
        {
            User =
                requestSettings?.User ??
                modelSettings?.User ??
                providerSettings?.User ??
                Default.User ??
                throw new InvalidOperationException("Default User is not set."),
            Endpoint =
                requestSettingsCasted?.Endpoint ??
                modelSettingsCasted?.Endpoint ??
                providerSettingsCasted?.Endpoint ??
                Default.Endpoint ??
                throw new InvalidOperationException("Default Endpoint is not set."),
        };
    }
}