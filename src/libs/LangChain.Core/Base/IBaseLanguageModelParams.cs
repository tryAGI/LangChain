namespace LangChain.NET.Base;

public interface IBaseLanguageModelParams : IBaseLangChainParams
{
    public string ApiKey { get; set; }
}