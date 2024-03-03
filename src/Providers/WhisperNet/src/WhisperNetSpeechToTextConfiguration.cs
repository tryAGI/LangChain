namespace LangChain.Providers.WhisperNet;

public class WhisperNetSpeechToTextConfiguration
{
    /// <summary>
    /// Path to *.bin file
    /// </summary>
    public string PathToModelFile { get; set; } = string.Empty;
    
    /// <summary>
    ///  
    /// </summary>
    public float? Temperature { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? Language { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? Prompt { get; init; }
}