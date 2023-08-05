using LangChain.Chat;
using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

public class AiMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public AiMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    public override async Task<BaseChatMessage> Format(InputValues values)
    {
        return new AiChatMessage(await this.Prompt.Format(values));
    }

    public static AiMessagePromptTemplate FromTemplate(string template)
    {
        return new AiMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}