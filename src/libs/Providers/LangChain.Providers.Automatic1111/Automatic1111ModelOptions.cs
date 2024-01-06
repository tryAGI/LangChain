namespace StableDiffusion;

/// <summary>
/// 
/// </summary>
public class Automatic1111ModelOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string NegativePrompt { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    public int Seed { get; set; } = -1;
    
    /// <summary>
    /// 
    /// </summary>
    public int Steps { get; set; } = 20;
    
    /// <summary>
    /// 
    /// </summary>
    public float CfgScale { get; set; } = 6.0F;
    
    /// <summary>
    /// 
    /// </summary>
    public int Width { get; set; } = 512;
    
    /// <summary>
    /// 
    /// </summary>
    public int Height { get; set; } = 512;
    
    /// <summary>
    /// 
    /// </summary>
    public string Sampler { get; set; } = "Euler a";
}
