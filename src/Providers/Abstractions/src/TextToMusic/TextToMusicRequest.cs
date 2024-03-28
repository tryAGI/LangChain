// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for image generation requests.
/// </summary>
public class TextToMusicRequest
{
    public required string Prompt { get; init; }
    
    /// <inheritdoc cref="ToTextToMusicRequest(string)"/>
    public static implicit operator TextToMusicRequest(string message)
    {
        return ToTextToMusicRequest(message);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="TextToImageRequest"/>. <br/>
    /// Will be converted to a <see cref="TextToImageRequest"/>
    /// with a prompt and default settings.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TextToMusicRequest ToTextToMusicRequest(string message)
    {
        return new TextToMusicRequest
        {
            Prompt = message,
        };
    }
}