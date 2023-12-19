﻿namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class GenerativeAiConfiguration
{

    /// <summary>
    /// Default configuration according to the official API documentation. <br/>
    /// </summary>
    public static GenerativeAiConfiguration Default { get; } = new()
    {

    };

    /// <summary>
    /// 
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// ID of the model to use. <br/>
    /// </summary>
    public string? ModelId { get; set; } = "gemini-pro";

    /// <inheritdoc cref="GenerateTextRequestParameters.Top_k"/>
    public int? TopK { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Top_p"/>
    public double? TopP { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Temperature"/>
    public double? Temperature { get; set; } = 1D;

    /// <summary>
    /// Maximum Output Tokens
    /// </summary>
    public int? MaxOutputTokens { get; set; } = default!;
   
}