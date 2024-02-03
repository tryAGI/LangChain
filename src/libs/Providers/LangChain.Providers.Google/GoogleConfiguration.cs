namespace LangChain.Providers.Google;

/// <summary>
/// 
/// </summary>
public class GoogleConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// ID of the model to use. <br/>
    /// </summary>
    public string? ModelId { get; set; } = "gemini-pro";

    /// <summary>
    /// 
    /// </summary>
    public int? TopK { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public double? TopP { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public double? Temperature { get; set; } = 1D;

    /// <summary>
    /// Maximum Output Tokens
    /// </summary>
    public int? MaxOutputTokens { get; set; } = default!;
   
}