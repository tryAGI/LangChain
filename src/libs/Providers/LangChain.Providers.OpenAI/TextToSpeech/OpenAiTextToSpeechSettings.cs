using OpenAI.Audio;
using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <summary>
/// According https://platform.openai.com/docs/guides/text-to-speech.
/// </summary>
[CLSCompliant(false)]
public readonly record struct OpenAiTextToSpeechSettings(
    TextToSpeechModels Model,
    SpeechVoice Voice,
    SpeechResponseFormat ResponseFormat,
    float Speed)
{
    /// <inheritdoc cref="OpenAiTextToSpeechSettings"/>
    public static OpenAiTextToSpeechSettings Default { get; } = new(
        Model: TextToSpeechModels.Tts1,
        Voice: SpeechVoice.Alloy,
        ResponseFormat: SpeechResponseFormat.MP3,
        Speed: 1.0F);
}