namespace LangChain.Providers;

/// <summary>
/// Text-to-image request.
/// </summary>
public class TextToImageRequest
{
    public required string Prompt { get; init; }

    /// <inheritdoc cref="ToTextToImageRequest(string)"/>
    public static implicit operator TextToImageRequest(string message)
    {
        return ToTextToImageRequest(message);
    }

    /// <summary>
    /// Converts a string to a <see cref="TextToImageRequest"/>.
    /// </summary>
    public static TextToImageRequest ToTextToImageRequest(string message)
    {
        return new TextToImageRequest
        {
            Prompt = message,
        };
    }
}
