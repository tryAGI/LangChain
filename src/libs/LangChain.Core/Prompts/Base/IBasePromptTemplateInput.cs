using LangChain.Schema;

namespace LangChain.Prompts.Base;

using System.Collections.Generic;

public interface IBasePromptTemplateInput
{
    List<string> InputVariables { get; }
    Dictionary<string, object> PartialVariables { get; }
}
