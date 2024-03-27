using OpenAI.Audio;
using OpenAI.Constants;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

/// <summary>
/// According https://platform.openai.com/docs/guides/speech-to-text.
/// According https://platform.openai.com/docs/api-reference/audio/createTranscription.
/// </summary>
public class OpenAiSpeechToTextSettings : SpeechToTextSettings
{
    /// <summary>
    /// 
    /// </summary>
    public new static OpenAiSpeechToTextSettings Default { get; } = new()
    {
        Model = SpeechToTextModels.Whisper1,
        AudioName = string.Empty,
        Prompt = string.Empty,
        ResponseFormat = AudioResponseFormat.Json,
        Temperature = 0,
        Language = string.Empty,
        MaxDuration = 0,
        UseAutomaticPunctuation = false,
    };

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public SpeechToTextModels? Model { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? AudioName { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? Prompt { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public AudioResponseFormat? ResponseFormat { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public float? Temperature { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// The maximum duration of the audio in seconds.
    /// </summary>
    public int? MaxDuration { get; init; }

    /// <summary>
    /// Whether to use automatic punctuation.
    /// </summary>
    public bool? UseAutomaticPunctuation { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OpenAiSpeechToTextSettings Calculate(
        SpeechToTextSettings? requestSettings,
        SpeechToTextSettings? modelSettings,
        SpeechToTextSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as OpenAiSpeechToTextSettings;
        var modelSettingsCasted = modelSettings as OpenAiSpeechToTextSettings;
        var providerSettingsCasted = providerSettings as OpenAiSpeechToTextSettings;

        return new OpenAiSpeechToTextSettings
        {
            Model =
                requestSettingsCasted?.Model ??
                modelSettingsCasted?.Model ??
                providerSettingsCasted?.Model ??
                Default.Model ??
                throw new InvalidOperationException("Default Model is not set."),
            AudioName =
                requestSettingsCasted?.AudioName ??
                modelSettingsCasted?.AudioName ??
                providerSettingsCasted?.AudioName ??
                Default.AudioName ??
                throw new InvalidOperationException("Default AudioName is not set."),
            Prompt =
                requestSettingsCasted?.Prompt ??
                modelSettingsCasted?.Prompt ??
                providerSettingsCasted?.Prompt ??
                Default.Prompt ??
                throw new InvalidOperationException("Default Prompt is not set."),
            ResponseFormat =
                requestSettingsCasted?.ResponseFormat ??
                modelSettingsCasted?.ResponseFormat ??
                providerSettingsCasted?.ResponseFormat ??
                Default.ResponseFormat ??
                throw new InvalidOperationException("Default ResponseFormat is not set."),
            Temperature =
                requestSettingsCasted?.Temperature ??
                modelSettingsCasted?.Temperature ??
                providerSettingsCasted?.Temperature ??
                Default.Temperature ??
                throw new InvalidOperationException("Default Temperature is not set."),
            Language =
                requestSettingsCasted?.Language ??
                modelSettingsCasted?.Language ??
                providerSettingsCasted?.Language ??
                Default.Language ??
                throw new InvalidOperationException("Default Language is not set."),
            MaxDuration =
                requestSettingsCasted?.MaxDuration ??
                modelSettingsCasted?.MaxDuration ??
                providerSettingsCasted?.MaxDuration ??
                Default.MaxDuration ??
                throw new InvalidOperationException("Default MaxDuration is not set."),
            UseAutomaticPunctuation =
                requestSettingsCasted?.UseAutomaticPunctuation ??
                modelSettingsCasted?.UseAutomaticPunctuation ??
                providerSettingsCasted?.UseAutomaticPunctuation ??
                Default.UseAutomaticPunctuation ??
                throw new InvalidOperationException("Default UseAutomaticPunctuation is not set."),
        };
    }
}