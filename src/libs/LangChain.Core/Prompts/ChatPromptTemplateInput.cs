using LangChain.Prompts.Base;

namespace LangChain.Prompts;

public class ChatPromptTemplateInput : IBasePromptTemplateInput
{
    public List<BaseMessagePromptTemplate> PromptMessages { get; set; }

    public bool ValidateTemplate { get; set; } = true;
    public IReadOnlyList<string> InputVariables { get; set; } = new List<string>();
    public Dictionary<string, object> PartialVariables { get; set; } = new();
}