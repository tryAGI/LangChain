using OpenAI.Constants;
using OpenAI.Images;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class OpenAiTextToImageSettings : TextToImageSettings
{
    /// <summary>
    /// 
    /// </summary>
    public static OpenAiTextToImageSettings GetDefaultSettings(string id)
    {
        if (id == ImageModels.DallE2)
        {
            return new OpenAiTextToImageSettings()
            {
                NumberOfResults = 1,
                Quality = ImageQualities.Standard,
                ResponseFormat = global::OpenAI.Images.ResponseFormat.B64_Json,
                Resolution = ImageResolutions._256x256,
                User = string.Empty,
            };
        }
        if (id == ImageModels.DallE3)
        {
            return new OpenAiTextToImageSettings()
            {
                NumberOfResults = 1,
                Quality = ImageQualities.Standard,
                ResponseFormat = global::OpenAI.Images.ResponseFormat.B64_Json,
                Resolution = ImageResolutions._1024x1024,
                User = string.Empty,
            };
        }

        throw new NotSupportedException($"OpenAI model {id} is not supported");
    }


    /// <summary>
    /// 
    /// </summary>
    public int? NumberOfResults { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public ImageQualities? Quality { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public ResponseFormat? ResponseFormat { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public ImageResolutions? Resolution { get; init; }

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
    /// <param name="defaultSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OpenAiTextToImageSettings Calculate(
        TextToImageSettings? requestSettings,
        TextToImageSettings? modelSettings,
        TextToImageSettings? providerSettings,
        TextToImageSettings? defaultSettings)
    {
        var requestSettingsCasted = requestSettings as OpenAiTextToImageSettings;
        var modelSettingsCasted = modelSettings as OpenAiTextToImageSettings;
        var providerSettingsCasted = providerSettings as OpenAiTextToImageSettings;
        var defaultSettingsCasted = defaultSettings as OpenAiTextToImageSettings;

        return new OpenAiTextToImageSettings
        {
            NumberOfResults =
                requestSettingsCasted?.NumberOfResults ??
                modelSettingsCasted?.NumberOfResults ??
                providerSettingsCasted?.NumberOfResults ??
                defaultSettingsCasted?.NumberOfResults ??
                throw new InvalidOperationException("Default NumberOfResults is not set."),
            Quality =
                requestSettingsCasted?.Quality ??
                modelSettingsCasted?.Quality ??
                providerSettingsCasted?.Quality ??
                defaultSettingsCasted?.Quality ??
                throw new InvalidOperationException("Default Quality is not set."),
            ResponseFormat =
                requestSettingsCasted?.ResponseFormat ??
                modelSettingsCasted?.ResponseFormat ??
                providerSettingsCasted?.ResponseFormat ??
                defaultSettingsCasted?.ResponseFormat ??
                throw new InvalidOperationException("Default ResponseFormat is not set."),
            Resolution =
                requestSettingsCasted?.Resolution ??
                modelSettingsCasted?.Resolution ??
                providerSettingsCasted?.Resolution ??
                defaultSettingsCasted?.Resolution ??
                throw new InvalidOperationException("Default Resolution is not set."),
            User =
                requestSettingsCasted?.User ??
                modelSettingsCasted?.User ??
                providerSettingsCasted?.User ??
                defaultSettingsCasted?.User ??
                throw new InvalidOperationException("Default User is not set."),
        };
    }
}