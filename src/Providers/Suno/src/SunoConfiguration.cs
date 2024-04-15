namespace LangChain.Providers.Suno;

/// <summary>
/// 
/// </summary>
public class SunoConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public const string SectionName = "Suno";

    /// <summary>
    /// 
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? StagingApiKey { get; set; }
}