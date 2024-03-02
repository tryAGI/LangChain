// ReSharper disable once CheckNamespace
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace LangChain.Providers;

#pragma warning disable CA2225

/// <summary>
/// 
/// </summary>
public class ImageToTextResponse
{
    /// <summary>
    /// 
    /// </summary>
    public required ImageToTextSettings UsedSettings { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Usage Usage { get; init; } = Usage.Empty;
    

    /// <summary>
    /// Generated text
    /// </summary>
    public string? Text { get; set; }
}