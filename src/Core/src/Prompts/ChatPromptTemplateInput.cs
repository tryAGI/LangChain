using LangChain.Prompts.Base;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class ChatPromptTemplateInput : IBasePromptTemplateInput
{
    /// <summary>
    /// 
    /// </summary>
    public List<BaseMessagePromptTemplate> PromptMessages { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public bool ValidateTemplate { get; set; } = true;

    /// <inheritdoc/>
    public IReadOnlyList<string> InputVariables { get; set; } = new List<string>();

    /// <inheritdoc/>
    public Dictionary<string, object> PartialVariables { get; set; } = new();
}