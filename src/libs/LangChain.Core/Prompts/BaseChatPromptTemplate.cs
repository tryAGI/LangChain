using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public abstract class BaseChatPromptTemplate : BasePromptTemplate
{
    protected BaseChatPromptTemplate(IBasePromptTemplateInput input) : base(input) { }

    public abstract Task<IReadOnlyCollection<Message>> FormatMessages(InputValues values);

    public override async Task<string> Format(InputValues values)
    {
        return (await this.FormatPromptValue(values).ConfigureAwait(false)).ToString();
    }

    public override async Task<BasePromptValue> FormatPromptValue(InputValues values)
    {
        var resultMessages = await this.FormatMessages(values).ConfigureAwait(false);
        return new ChatPromptValue(resultMessages);
    }
}