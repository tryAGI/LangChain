using OpenAI.Audio;
using OpenAI.Constants;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

/// <summary>
/// According https://platform.openai.com/docs/guides/text-to-speech.
/// </summary>
public class OpenAiTextToSpeechSettings : TextToSpeechSettings
{
    /// <inheritdoc cref="OpenAiTextToSpeechSettings"/>
    public new static OpenAiTextToSpeechSettings Default { get; } = new()
    {
        Model = TextToSpeechModels.Tts1,
        Voice = SpeechVoice.Alloy,
        ResponseFormat = SpeechResponseFormat.MP3,
        Speed = 1.0F,
    };
    
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public TextToSpeechModels? Model { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public SpeechVoice? Voice { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public SpeechResponseFormat? ResponseFormat { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public float? Speed { get; init; }

    /// <summary>
    /// The pitch of the voice, allowing for higher or lower tones.
    /// </summary>
    public float? Pitch { get; init; }

    /// <summary>
    /// The emphasis of the speech, affecting how expressive the voice sounds.
    /// </summary>
    public SpeechEmphasis? Emphasis { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static OpenAiTextToSpeechSettings Calculate(
        TextToSpeechSettings? requestSettings,
        TextToSpeechSettings? modelSettings,
        TextToSpeechSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as OpenAiTextToSpeechSettings;
        var modelSettingsCasted = modelSettings as OpenAiTextToSpeechSettings;
        var providerSettingsCasted = providerSettings as OpenAiTextToSpeechSettings;

        return new OpenAiTextToSpeechSettings
        {
            Model =
                requestSettingsCasted?.Model ??
                modelSettingsCasted?.Model ??
                providerSettingsCasted?.Model ??
                Default.Model ??
                throw new InvalidOperationException("Default Model is not set."),
            Voice =
                requestSettingsCasted?.Voice ??
                modelSettingsCasted?.Voice ??
                providerSettingsCasted?.Voice ??
                Default.Voice ??
                throw new InvalidOperationException("Default Voice is not set."),
            ResponseFormat =
                requestSettingsCasted?.ResponseFormat ??
                modelSettingsCasted?.ResponseFormat ??
                providerSettingsCasted?.ResponseFormat ??
                Default.ResponseFormat ??
                throw new InvalidOperationException("Default ResponseFormat is not set."),
            Speed =
                requestSettingsCasted?.Speed ??
                modelSettingsCasted?.Speed ??
                providerSettingsCasted?.Speed ??
                Default.Speed ??
                throw new InvalidOperationException("Default Speed is not set."),
            Pitch =
                requestSettingsCasted?.Pitch ??
                modelSettingsCasted?.Pitch ??
                providerSettingsCasted?.Pitch ??
                Default.Pitch ??
                throw new InvalidOperationException("Default Pitch is not set."),
            Emphasis =
                requestSettingsCasted?.Emphasis ??
                modelSettingsCasted?.Emphasis ??
                providerSettingsCasted?.Emphasis ??
                Default.Emphasis ??
                throw new InvalidOperationException("Default Emphasis is not set."),
        };
    }
}