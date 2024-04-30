// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for image generation requests.
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
    /// Explicitly converts a string to a <see cref="TextToImageRequest"/>. <br/>
    /// Will be converted to a <see cref="TextToImageRequest"/>
    /// with a prompt and default settings.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TextToImageRequest ToTextToImageRequest(string message)
    {
        return new TextToImageRequest
        {
            Prompt = message,
        };
    }
}