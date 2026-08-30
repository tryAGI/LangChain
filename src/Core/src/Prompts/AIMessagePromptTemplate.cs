using LangChain.Prompts.Base;
using LangChain.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class AiMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    /// <inheritdoc/>
    public AiMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    /// <inheritdoc/>
    public override async Task<ChatMessage> FormatAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        return new ChatMessage(ChatRole.Assistant, await Prompt.FormatAsync(values, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static AiMessagePromptTemplate FromTemplate(string template)
    {
        return new AiMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}
