namespace LangChain.Base;

/// <summary>
/// 
/// </summary>
public interface IBaseLangChainParams
{
    /// <summary>
    /// Whether or not run in verbose mode. In verbose mode, some intermediate logs
    /// will be printed to the console. 
    /// </summary>
    bool Verbose { get; set; }
}