// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for text-to-speech requests.
/// </summary>
public class TextToSpeechRequest
{
    public required string Prompt { get; init; }
    
    /// <inheritdoc cref="ToTextToSpeechRequest(string)"/>
    public static implicit operator TextToSpeechRequest(string message)
    {
        return ToTextToSpeechRequest(message);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="TextToSpeechRequest"/>. <br/>
    /// Will be converted to a <see cref="TextToSpeechRequest"/>
    /// with a prompt and default settings.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TextToSpeechRequest ToTextToSpeechRequest(string message)
    {
        return new TextToSpeechRequest
        {
            Prompt = message,
        };
    }
}