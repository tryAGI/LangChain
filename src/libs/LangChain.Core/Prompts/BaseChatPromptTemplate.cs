using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

/// <inheritdoc/>
public abstract class BaseChatPromptTemplate : BasePromptTemplate
{
    /// <inheritdoc/>
    protected BaseChatPromptTemplate(IBasePromptTemplateInput input) : base(input) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public abstract Task<IReadOnlyCollection<Message>> FormatMessages(InputValues values);

    /// <inheritdoc/>
    public override async Task<string> Format(InputValues values)
    {
        return (await this.FormatPromptValue(values).ConfigureAwait(false)).ToString();
    }

    /// <inheritdoc/>
    public override async Task<BasePromptValue> FormatPromptValue(InputValues values)
    {
        var resultMessages = await this.FormatMessages(values).ConfigureAwait(false);
        return new ChatPromptValue(resultMessages);
    }
}