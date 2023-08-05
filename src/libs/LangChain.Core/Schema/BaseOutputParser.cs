using LangChain.Callback;

namespace LangChain.Schema;

using System;

public abstract class BaseOutputParser<T>
{
    public abstract Task<T> Parse(string? text, CallbackManager? callbacks = null);

    public virtual async Task<T> ParseWithPrompt(string? text, BasePromptValue prompt, CallbackManager? callbacks = null)
    {
        return await Parse(text, callbacks);
    }

    public abstract string GetFormatInstructions();

    protected virtual string _type()
    {
        throw new NotImplementedException("_type not implemented");
    }
}