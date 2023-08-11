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
    List<string> InputVariables { get; }
    
    /// <summary>
    /// 
    /// </summary>
    Dictionary<string, object> PartialVariables { get; }
}
