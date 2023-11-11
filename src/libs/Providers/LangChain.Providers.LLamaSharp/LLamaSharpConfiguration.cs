namespace LangChain.Providers.LLamaSharp;

public class LLamaSharpConfiguration
{
    /// <summary>
    /// Path to *.bin file
    /// </summary>
    public string? PathToModelFile { get; set; }

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

    public int Seed { get; set; }

    public int MaxTokens { get; set; } = 600;

}