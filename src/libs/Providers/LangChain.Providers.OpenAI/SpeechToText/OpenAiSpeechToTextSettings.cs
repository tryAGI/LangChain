using OpenAI.Audio;
using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <summary>
/// According https://platform.openai.com/docs/guides/speech-to-text.
/// According https://platform.openai.com/docs/api-reference/audio/createTranscription.
/// </summary>
[CLSCompliant(false)]
public readonly record struct OpenAiSpeechToTextSettings(
    SpeechToTextModel Model,
    string AudioName,
    string Prompt,
    AudioResponseFormat ResponseFormat,
    float Temperature,
    string Language)
{
    /// <inheritdoc cref="OpenAiSpeechToTextSettings"/>
    public static OpenAiSpeechToTextSettings Default { get; } = new(
        Model: SpeechToTextModel.Whisper1,
        AudioName: string.Empty,
        Prompt: string.Empty,
        ResponseFormat: AudioResponseFormat.Json,
        Temperature: 0,
        Language: string.Empty);
}