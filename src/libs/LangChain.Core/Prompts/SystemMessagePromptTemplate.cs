using LangChain.Chat;
using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

public class SystemMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public SystemMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    public override async Task<BaseChatMessage> Format(InputValues values)
    {
        return new SystemChatMessage(await this.Prompt.Format(values));
    }

    public static SystemMessagePromptTemplate FromTemplate(string template)
    {
        return new SystemMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}