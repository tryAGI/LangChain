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
    public double CfgScale { get; set; } = 7;
    
    /// <summary>
    /// 
    /// </summary>
    public int Width { get; set; } = 512;
    
    /// <summary>
    /// 
    /// </summary>
    public int Height { get; set; } = 512;


}