using LangChain.NET.Chat;
using LangChain.NET.Prompts.Base;
using LangChain.NET.Schema;

namespace LangChain.NET.Prompts;

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