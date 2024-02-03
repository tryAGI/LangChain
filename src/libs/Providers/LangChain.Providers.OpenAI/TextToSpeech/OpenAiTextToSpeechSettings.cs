using OpenAI.Audio;
using OpenAI.Constants;
using System.Diagnostics.CodeAnalysis;

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
    [MemberNotNull(nameof(Model))]
    [CLSCompliant(false)]
    public TextToSpeechModels? Model { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Voice))]
    [CLSCompliant(false)]
    public SpeechVoice? Voice { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(ResponseFormat))]
    [CLSCompliant(false)]
    public SpeechResponseFormat? ResponseFormat { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Speed))]
    public float? Speed { get; init; }
    
}