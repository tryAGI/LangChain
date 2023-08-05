using LangChain.NET.Base;
using LangChain.NET.Cache;

namespace LangChain.NET.LLMS;

public interface IBaseLlmParams : IBaseLanguageModelParams
{
    internal decimal? Concurrency { get; set; }
    
    internal BaseCache? Cache { get; set; }
}