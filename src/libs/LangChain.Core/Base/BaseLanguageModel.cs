using LangChain.NET.Schema;

namespace LangChain.NET.Base;

public abstract class BaseLanguageModel : BaseLangChain
{
    public BaseLanguageModel(IBaseLanguageModelParams parameters) : base(parameters)
    {
    }

    public abstract string ModelType { get; set; }

    public abstract string LlmType { get; set; }

    public abstract Task<LlmResult> GeneratePrompt(BasePromptValue[] promptValues, List<string>? stop = null);
}