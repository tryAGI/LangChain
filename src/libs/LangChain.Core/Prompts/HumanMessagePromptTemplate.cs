using LangChain.NET.Chat;
using LangChain.NET.Prompts.Base;
using LangChain.NET.Schema;

namespace LangChain.NET.Prompts;

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