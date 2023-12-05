using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public class AiMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    public AiMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    public override async Task<Message> Format(InputValues values)
    {
        return (await this.Prompt.Format(values).ConfigureAwait(false)).AsAiMessage();
    }

    public static AiMessagePromptTemplate FromTemplate(string template)
    {
        return new AiMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}