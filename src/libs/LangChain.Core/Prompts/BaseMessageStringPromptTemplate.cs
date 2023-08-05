using LangChain.Chat;
using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

public abstract class BaseMessageStringPromptTemplate : BaseMessagePromptTemplate
{
    public BaseStringPromptTemplate Prompt { get; set; }

    protected BaseMessageStringPromptTemplate(BaseStringPromptTemplate prompt)
    {
        this.Prompt = prompt;
    }

    public override List<string> InputVariables => this.Prompt.InputVariables;

    public abstract Task<BaseChatMessage> Format(InputValues values);

    public override async Task<List<BaseChatMessage>> FormatMessages(InputValues values)
    {
        return new List<BaseChatMessage> { await this.Format(values) };
    }
}