namespace LangChain.Providers.LLamaSharp;

public class LLamaSharpConfiguration
{
    /// <summary>
    /// Path to *.bin file
    /// </summary>
    public string PathToModelFile { get; set; }
    
    /// <summary>
    /// Model mode.
    /// Chat - for conversation completion
    /// Instruction - for instruction execution
    /// </summary>
    public ELLamaSharpModelMode Mode { get; set; } = ELLamaSharpModelMode.Chat;
    
    /// <summary>
    /// Context size
    /// How much tokens model will remember.
    /// Usually 2048 for llama
    /// </summary>
    public int ContextSize { get; set; } = 512;

    /// <summary>
    /// Temperature
    /// The level of model's creativity
    /// </summary>
    public float Temperature { get; set; } = 0.7f;

}