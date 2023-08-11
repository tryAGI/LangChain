using LangChain.Callback;

namespace LangChain.Schema;

using System;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseOutputParser<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="callbacks"></param>
    /// <returns></returns>
    public abstract Task<T> Parse(string? text, CallbackManager? callbacks = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="prompt"></param>
    /// <param name="callbacks"></param>
    /// <returns></returns>
    public virtual async Task<T> ParseWithPrompt(string? text, BasePromptValue prompt, CallbackManager? callbacks = null)
    {
        return await Parse(text, callbacks);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract string GetFormatInstructions();

    protected virtual string _type()
    {
        throw new NotImplementedException("_type not implemented");
    }
}