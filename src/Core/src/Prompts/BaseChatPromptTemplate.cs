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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IReadOnlyCollection<Message>> FormatMessagesAsync(
        InputValues values,
        CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public override async Task<string> FormatAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        return (await FormatPromptValueAsync(values, cancellationToken).ConfigureAwait(false)).ToString();
    }

    /// <inheritdoc/>
    public override async Task<BasePromptValue> FormatPromptValueAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        var resultMessages = await FormatMessagesAsync(values, cancellationToken).ConfigureAwait(false);
        return new ChatPromptValue(resultMessages);
    }
}