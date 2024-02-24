namespace LangChain.Base;

/// <inheritdoc />
public interface IBaseLanguageModelParams : IBaseLangChainParams
{
    /// <summary>
    /// 
    /// </summary>
    public string ApiKey { get; set; }
}