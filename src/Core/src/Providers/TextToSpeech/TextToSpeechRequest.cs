namespace LangChain.Providers;

/// <summary>
/// Text-to-speech request.
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
    /// Converts a string to a <see cref="TextToSpeechRequest"/>.
    /// </summary>
    public static TextToSpeechRequest ToTextToSpeechRequest(string message)
    {
        return new TextToSpeechRequest
        {
            Prompt = message,
        };
    }
}
