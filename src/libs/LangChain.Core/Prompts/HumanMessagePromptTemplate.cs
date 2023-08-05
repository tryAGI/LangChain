using LangChain.Chat;
using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

public class HumanMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public HumanMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    public override async Task<BaseChatMessage> Format(InputValues values)
    {
        return new HumanChatMessage(await this.Prompt.Format(values));
    }

    public static HumanMessagePromptTemplate FromTemplate(string template)
    {
        return new HumanMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}