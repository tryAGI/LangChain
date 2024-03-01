// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for chat requests.
/// </summary>
public class ImageToTextRequest
{
    /// <summary>
    /// Defines the messages for the request.
    /// </summary>
    public required BinaryData Image { get; init; }
    
}