using LangChain.Schema;

namespace LangChain.Prompts;

public class PromptTemplateInput : IPromptTemplateInput
{
    public PromptTemplateInput(string template, IReadOnlyList<string> inputVariables, Dictionary<string, object> partialVariables = null)
    {
        this.Template = template;
        this.InputVariables = inputVariables;
        this.PartialVariables = partialVariables ?? new();
    }

    public string Template { get; private set; }
    public TemplateFormatOptions? TemplateFormat { get; set; }

    public bool? ValidateTemplate { get; set; }

    public IReadOnlyList<string> InputVariables { get; private set; }

    public Dictionary<string, object> PartialVariables { get; private set; }
}