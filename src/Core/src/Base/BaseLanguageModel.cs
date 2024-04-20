using LangChain.Schema;

namespace LangChain.Base;

/// <inheritdoc />
public abstract class BaseLanguageModel : BaseLangChain
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    protected BaseLanguageModel(IBaseLanguageModelParams parameters) : base(parameters)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract string ModelType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public abstract string LlmType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="promptValues"></param>
    /// <param name="stopSequences"></param>
    /// <returns></returns>
    public abstract Task<LlmResult> GeneratePrompt(BasePromptValue[] promptValues, IReadOnlyCollection<string>? stopSequences = null);
}