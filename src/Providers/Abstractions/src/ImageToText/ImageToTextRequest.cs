// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for image to text requests.
/// </summary>
public class ImageToTextRequest
{
    /// <summary>
    /// Image to upload.
    /// </summary>
    public required BinaryData Image { get; init; }
}