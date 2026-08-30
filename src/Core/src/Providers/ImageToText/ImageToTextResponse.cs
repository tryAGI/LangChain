namespace LangChain.Providers;

/// <summary>
/// Image-to-text response.
/// </summary>
public class ImageToTextResponse
{
    /// <summary>
    /// The settings used for this request.
    /// </summary>
    public required ImageToTextSettings UsedSettings { get; init; }

    /// <summary>
    /// Usage information.
    /// </summary>
    public Usage Usage { get; init; } = Usage.Empty;

    /// <summary>
    /// Generated text.
    /// </summary>
    public string? Text { get; set; }
}
