using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public class ChatMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public string Role { get; set; }

    public ChatMessagePromptTemplate(BaseStringPromptTemplate prompt, string role) : base(prompt)
    {
        this.Role = role;
    }

    public override async Task<Message> Format(InputValues values)
    {
        return (await this.Prompt.Format(values).ConfigureAwait(false)).AsChatMessage();
    }

    public static ChatMessagePromptTemplate FromTemplate(string template, string role)
    {
        return new ChatMessagePromptTemplate(PromptTemplate.FromTemplate(template), role);
    }
}