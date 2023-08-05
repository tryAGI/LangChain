using LangChain.NET.Schema;

namespace LangChain.NET.Prompts.Base;

using System.Collections.Generic;

public interface IBasePromptTemplateInput
{
    List<string> InputVariables { get; }
    Dictionary<string, object> PartialVariables { get; }
}
