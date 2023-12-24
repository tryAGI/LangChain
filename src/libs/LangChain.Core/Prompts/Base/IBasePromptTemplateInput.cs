using LangChain.Schema;

namespace LangChain.Prompts.Base;

using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public interface IBasePromptTemplateInput
{
    /// <summary>
    /// 
    /// </summary>
    IReadOnlyList<string> InputVariables { get; }

    /// <summary>
    /// 
    /// </summary>
    Dictionary<string, object> PartialVariables { get; }
}
