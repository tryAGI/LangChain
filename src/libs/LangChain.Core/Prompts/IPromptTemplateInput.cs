using LangChain.Prompts.Base;

namespace LangChain.Prompts;

/// <inheritdoc/>
public interface IPromptTemplateInput : IBasePromptTemplateInput
{
    /// <summary>
    /// 
    /// </summary>
    string Template { get; }

    /// <summary>
    /// 
    /// </summary>
    TemplateFormatOptions? TemplateFormat { get; }

    /// <summary>
    /// 
    /// </summary>
    bool? ValidateTemplate { get; }
}