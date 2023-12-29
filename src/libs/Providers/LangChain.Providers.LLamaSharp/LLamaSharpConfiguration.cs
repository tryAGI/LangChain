namespace LangChain.Providers.LLamaSharp;

/// <summary>
/// 
/// </summary>
public class LLamaSharpConfiguration
{
    /// <summary>
    /// Path to *.bin file
    /// </summary>
    public string PathToModelFile { get; set; } = string.Empty;

    /// <summary>
    /// Context size
    /// How much tokens model will remember.
    /// Usually 2048 for llama
    /// </summary>
    public int ContextSize { get; set; } = 1024;

    /// <summary>
    /// Temperature
    /// The level of model's creativity
    /// </summary>
    public float Temperature { get; set; } = 0.7f;

    /// <summary>
    /// 
    /// </summary>
    public int Seed { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int MaxTokens { get; set; } = 600;

    public float RepeatPenalty { get; set; } = 1;

    public List<string> AntiPrompts { get; set; } = new() { ">", "Human: "};

    

}