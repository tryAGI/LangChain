// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class ImageData
{
    public string? Base64 { get; init; }
    
    /// <inheritdoc cref="ToImageData(string)"/>
    public static implicit operator ImageData(string message)
    {
        return ToImageData(message);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="ImageData"/>. <br/>
    /// Will be converted to a <see cref="ImageData"/>
    /// with non null Base64 property.
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static ImageData ToImageData(string base64)
    {
        return new ImageData
        {
            Base64 = base64,
        };
    }
}