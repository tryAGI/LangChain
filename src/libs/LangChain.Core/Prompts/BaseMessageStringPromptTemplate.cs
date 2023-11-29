using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public abstract class BaseMessageStringPromptTemplate : BaseMessagePromptTemplate
{
    public BaseStringPromptTemplate Prompt { get; set; }

    protected BaseMessageStringPromptTemplate(BaseStringPromptTemplate prompt)
    {
        this.Prompt = prompt;
    }

    public override IReadOnlyList<string> InputVariables => this.Prompt.InputVariables;

    public abstract Task<Message> Format(InputValues values);

    public override async Task<List<Message>> FormatMessages(InputValues values)
    {
        return new List<Message> { await this.Format(values) };
    }
}