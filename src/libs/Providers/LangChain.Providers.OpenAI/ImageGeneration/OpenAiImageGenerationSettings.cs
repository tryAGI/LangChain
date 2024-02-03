using System.Diagnostics.CodeAnalysis;
using OpenAI.Constants;
using OpenAI.Images;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class OpenAiImageGenerationSettings : ImageGenerationSettings
{
    /// <summary>
    /// 
    /// </summary>
    public new static OpenAiImageGenerationSettings Default { get; } = new()
    {
        NumberOfResults = 1,
        Quality = ImageQualities.Standard,
        ResponseFormat = global::OpenAI.Images.ResponseFormat.B64_Json,
        Resolution = ImageResolutions._256x256,
        User = string.Empty,
    };

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(NumberOfResults))]
    public int? NumberOfResults { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Quality))]
    [CLSCompliant(false)]
    public ImageQualities? Quality { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(ResponseFormat))]
    [CLSCompliant(false)]
    public ResponseFormat? ResponseFormat { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Resolution))]
    [CLSCompliant(false)]
    public ImageResolutions? Resolution { get; init; }
        
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(User))]
    public string? User { get; init; }
}