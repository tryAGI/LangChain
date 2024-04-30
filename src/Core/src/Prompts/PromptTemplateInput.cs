namespace LangChain.Prompts;

/// <inheritdoc/>
public class PromptTemplateInput(
    string template,
    IReadOnlyList<string> inputVariables,
    Dictionary<string, object>? partialVariables = null)
    : IPromptTemplateInput
{
    /// <inheritdoc/>
    public string Template { get; private set; } = template;

    /// <inheritdoc/>
    public TemplateFormatOptions? TemplateFormat { get; set; }

    /// <inheritdoc/>
    public bool? ValidateTemplate { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<string> InputVariables { get; private set; } = inputVariables;

    /// <inheritdoc/>
    public Dictionary<string, object> PartialVariables { get; private set; } = partialVariables ?? new();
}