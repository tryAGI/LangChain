using LangChain.Chat;
using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

public abstract class BaseChatPromptTemplate : BasePromptTemplate
{
    protected BaseChatPromptTemplate(IBasePromptTemplateInput input) : base(input) { }

    public abstract Task<BaseChatMessage[]> FormatMessages(InputValues values);

    public override async Task<string> Format(InputValues values)
    {
        return (await this.FormatPromptValue(values)).ToString();
    }

    public override async Task<BasePromptValue> FormatPromptValue(InputValues values)
    {
        var resultMessages = await this.FormatMessages(values);
        return new ChatPromptValue(resultMessages);
    }
}