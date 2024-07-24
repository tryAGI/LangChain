// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for image to text requests.
/// </summary>
public class ImageToTextRequest
{
    /// <summary>
    /// Description of the image to generate.
    /// </summary>
    public string Prompt { get; init; } = string.Empty;

    /// <summary>
    /// Image to upload.
    /// </summary>
    public required BinaryData Image { get; init; }
}