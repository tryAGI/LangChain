using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class HumanMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    /// <inheritdoc/>
    public HumanMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    /// <inheritdoc/>
    public override async Task<Message> Format(InputValues values)
    {
        return (await this.Prompt.Format(values).ConfigureAwait(false)).AsHumanMessage();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static HumanMessagePromptTemplate FromTemplate(string template)
    {
        return new HumanMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}