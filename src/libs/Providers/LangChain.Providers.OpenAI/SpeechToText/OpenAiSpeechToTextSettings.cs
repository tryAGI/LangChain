using OpenAI.Audio;
using OpenAI.Constants;
using System.Diagnostics.CodeAnalysis;

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
    };

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Model))]
    [CLSCompliant(false)]
    public SpeechToTextModels? Model { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(AudioName))]
    public string? AudioName { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Prompt))]
    public string? Prompt { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(ResponseFormat))]
    [CLSCompliant(false)]
    public AudioResponseFormat? ResponseFormat { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Temperature))]
    public float? Temperature { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Language))]
    public string? Language { get; init; }
}