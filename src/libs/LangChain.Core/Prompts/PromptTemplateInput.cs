namespace LangChain.Prompts;

public class PromptTemplateInput(
    string template,
    IReadOnlyList<string> inputVariables,
    Dictionary<string, object>? partialVariables = null)
    : IPromptTemplateInput
{
    public string Template { get; private set; } = template;
    public TemplateFormatOptions? TemplateFormat { get; set; }

    public bool? ValidateTemplate { get; set; }

    public IReadOnlyList<string> InputVariables { get; private set; } = inputVariables;

    public Dictionary<string, object> PartialVariables { get; private set; } = partialVariables ?? new();
}