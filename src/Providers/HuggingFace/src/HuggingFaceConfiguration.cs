namespace LangChain.Providers.HuggingFace;

/// <summary>
/// 
/// </summary>
public class HuggingFaceConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public const string SectionName = "HuggingFace";

    /// <summary>
    /// 
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// ID of the model to use. <br/>
    /// </summary>
    public string? ModelId { get; set; }

    /// <inheritdoc cref="GenerateTextRequestParameters.Top_k"/>
    public int? TopK { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Top_p"/>
    public double? TopP { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Temperature"/>
    public double? Temperature { get; set; } = 1D;

    /// <inheritdoc cref="GenerateTextRequestParameters.Repetition_penalty"/>
    public double? RepetitionPenalty { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Max_new_tokens"/>
    public int? MaxNewTokens { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Max_time"/>
    public double? MaxTime { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Return_full_text"/>
    public object? ReturnFullText { get; set; } = default!;

    /// <inheritdoc cref="GenerateTextRequestParameters.Num_return_sequences"/>
    public int? NumReturnSequences { get; set; } = 1;

    /// <inheritdoc cref="GenerateTextRequestParameters.Do_sample"/>   
    public bool? DoSample { get; set; } = default!;
}