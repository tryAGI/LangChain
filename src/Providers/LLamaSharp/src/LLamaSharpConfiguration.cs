using LLama.Native;

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

    /// <summary>
    /// 
    /// </summary>
    public float RepeatPenalty { get; set; } = 1;

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<string> AntiPrompts { get; set; } = [">", "Human: "];

    /// <summary>
    /// 
    /// </summary>
    public int MainGpu { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public GPUSplitMode SplitMode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int GpuLayerCount { get; set; } = 20;

    /// <summary>
    /// 
    /// </summary>
    public bool UseMemorymap { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public bool UseMemoryLock { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public uint? Threads { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public uint? BatchThreads { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public uint BatchSize { get; set; } = 512u;

    /// <summary>
    /// 
    /// </summary>
    public bool EmbeddingMode { get; set; }
}