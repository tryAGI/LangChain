using OpenAI.Constants;
using OpenAI.Images;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class OpenAiImageGenerationSettings : ImageGenerationSettings
{
    /// <summary>
    /// 
    /// </summary>
    public new static OpenAiImageGenerationSettings Default { get; } = new()
    {
        NumberOfResults = 1,
        Quality = ImageQualities.Standard,
        ResponseFormat = global::OpenAI.Images.ResponseFormat.B64_Json,
        Resolution = ImageResolutions._256x256,
        User = string.Empty,
    };

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
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OpenAiImageGenerationSettings Calculate(
        ImageGenerationSettings? requestSettings,
        ImageGenerationSettings? modelSettings,
        ImageGenerationSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as OpenAiImageGenerationSettings;
        var modelSettingsCasted = modelSettings as OpenAiImageGenerationSettings;
        var providerSettingsCasted = providerSettings as OpenAiImageGenerationSettings;

        return new OpenAiImageGenerationSettings
        {
            NumberOfResults =
                requestSettingsCasted?.NumberOfResults ??
                modelSettingsCasted?.NumberOfResults ??
                providerSettingsCasted?.NumberOfResults ??
                Default.NumberOfResults ??
                throw new InvalidOperationException("Default NumberOfResults is not set."),
            Quality =
                requestSettingsCasted?.Quality ??
                modelSettingsCasted?.Quality ??
                providerSettingsCasted?.Quality ??
                Default.Quality ??
                throw new InvalidOperationException("Default Quality is not set."),
            ResponseFormat =
                requestSettingsCasted?.ResponseFormat ??
                modelSettingsCasted?.ResponseFormat ??
                providerSettingsCasted?.ResponseFormat ??
                Default.ResponseFormat ??
                throw new InvalidOperationException("Default ResponseFormat is not set."),
            Resolution =
                requestSettingsCasted?.Resolution ??
                modelSettingsCasted?.Resolution ??
                providerSettingsCasted?.Resolution ??
                Default.Resolution ??
                throw new InvalidOperationException("Default Resolution is not set."),
            User =
                requestSettingsCasted?.User ??
                modelSettingsCasted?.User ??
                providerSettingsCasted?.User ??
                Default.User ??
                throw new InvalidOperationException("Default User is not set."),
        };
    }
}