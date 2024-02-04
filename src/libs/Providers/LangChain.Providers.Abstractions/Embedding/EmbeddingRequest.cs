// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Base class for embedding requests.
/// </summary>
public class EmbeddingRequest
{
    public required IList<string> Strings { get; init; } = Array.Empty<string>();
    
    /// <inheritdoc cref="ToEmbeddingRequest(string)"/>
    public static implicit operator EmbeddingRequest(string message)
    {
        return ToEmbeddingRequest(message);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="EmbeddingRequest"/>. <br/>
    /// Will be converted to a <see cref="EmbeddingRequest"/>
    /// with a single string.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static EmbeddingRequest ToEmbeddingRequest(string message)
    {
        return new EmbeddingRequest
        {
            Strings = new[] { message },
        };
    }
}