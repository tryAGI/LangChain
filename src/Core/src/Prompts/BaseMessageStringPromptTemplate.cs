using LangChain.Prompts.Base;
using LangChain.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Prompts;

/// <inheritdoc/>
public abstract class BaseMessageStringPromptTemplate(
    BaseStringPromptTemplate prompt)
    : BaseMessagePromptTemplate
{
    /// <summary>
    ///
    /// </summary>
    public BaseStringPromptTemplate Prompt { get; set; } = prompt;

    /// <inheritdoc/>
    public override IReadOnlyList<string> InputVariables => this.Prompt.InputVariables;

    /// <summary>
    ///
    /// </summary>
    /// <param name="values"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<ChatMessage> FormatAsync(
        InputValues values,
        CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public override async Task<List<ChatMessage>> FormatMessagesAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        return [await FormatAsync(values, cancellationToken).ConfigureAwait(false)];
    }
}
