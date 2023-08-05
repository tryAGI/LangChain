using LangChain.Prompts.Base;

namespace LangChain.Prompts;

public interface IPromptTemplateInput : IBasePromptTemplateInput
{
    string Template { get; }
    
    TemplateFormatOptions? TemplateFormat { get; }
    
    bool? ValidateTemplate { get; }
}