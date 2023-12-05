using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public class HumanMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public HumanMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    public override async Task<Message> Format(InputValues values)
    {
        return (await this.Prompt.Format(values).ConfigureAwait(false)).AsHumanMessage();
    }

    public static HumanMessagePromptTemplate FromTemplate(string template)
    {
        return new HumanMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}