// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for image generation requests.
/// </summary>
public class ImageGenerationRequest
{
    public required string Prompt { get; init; }
    
    /// <inheritdoc cref="ToImageGenerationRequest(string)"/>
    public static implicit operator ImageGenerationRequest(string message)
    {
        return ToImageGenerationRequest(message);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="ImageGenerationRequest"/>. <br/>
    /// Will be converted to a <see cref="ImageGenerationRequest"/>
    /// with a prompt and default settings.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ImageGenerationRequest ToImageGenerationRequest(string message)
    {
        return new ImageGenerationRequest
        {
            Prompt = message,
        };
    }
}