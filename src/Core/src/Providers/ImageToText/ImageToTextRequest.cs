namespace LangChain.Providers;

/// <summary>
/// Image-to-text request.
/// </summary>
public class ImageToTextRequest
{
    /// <summary>
    /// Description/prompt for the image.
    /// </summary>
    public string Prompt { get; init; } = string.Empty;

    /// <summary>
    /// Image to process.
    /// </summary>
    public required BinaryData Image { get; init; }
}
