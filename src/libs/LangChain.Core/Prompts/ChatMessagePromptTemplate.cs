using LangChain.Chat;
using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

public class ChatMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public string Role { get; set; }

    public ChatMessagePromptTemplate(BaseStringPromptTemplate prompt, string role) : base(prompt)
    {
        this.Role = role;
    }

    public override async Task<BaseChatMessage> Format(InputValues values)
    {
        return new ChatMessage(await this.Prompt.Format(values));
    }

    public static ChatMessagePromptTemplate FromTemplate(string template, string role)
    {
        return new ChatMessagePromptTemplate(PromptTemplate.FromTemplate(template), role);
    }
}