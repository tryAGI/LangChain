using LangChain.NET.Prompts.Base;

namespace LangChain.NET.Prompts;

public interface IPromptTemplateInput : IBasePromptTemplateInput
{
    string Template { get; }
    
    TemplateFormatOptions? TemplateFormat { get; }
    
    bool? ValidateTemplate { get; }
}