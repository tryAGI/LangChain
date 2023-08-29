using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public class SystemMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public SystemMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    public override async Task<Message> Format(InputValues values)
    {
        return (await this.Prompt.Format(values)).AsSystemMessage();
    }

    public static SystemMessagePromptTemplate FromTemplate(string template)
    {
        return new SystemMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}