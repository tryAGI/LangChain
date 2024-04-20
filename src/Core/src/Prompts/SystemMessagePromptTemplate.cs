using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class SystemMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    /// <inheritdoc/>
    public SystemMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    /// <inheritdoc/>
    public override async Task<Message> Format(InputValues values)
    {
        return (await this.Prompt.Format(values).ConfigureAwait(false)).AsSystemMessage();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static SystemMessagePromptTemplate FromTemplate(string template)
    {
        return new SystemMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}