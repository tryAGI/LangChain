using OpenAI.Audio;
using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <summary>
/// According https://platform.openai.com/docs/guides/text-to-speech.
/// </summary>
public readonly record struct OpenAiTextToSpeechSettings(
    TextToSpeechModel Model,
    SpeechVoice Voice,
    SpeechResponseFormat ResponseFormat,
    float Speed)
{
    /// <inheritdoc cref="OpenAiTextToSpeechSettings"/>
    public static OpenAiTextToSpeechSettings Default { get; } = new(
        Model: TextToSpeechModel.Tts1,
        Voice: SpeechVoice.Alloy,
        ResponseFormat: SpeechResponseFormat.MP3,
        Speed: 1.0F);
}