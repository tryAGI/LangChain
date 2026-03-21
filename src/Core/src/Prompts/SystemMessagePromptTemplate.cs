using LangChain.Prompts.Base;
using LangChain.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class SystemMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    /// <inheritdoc/>
    public SystemMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    /// <inheritdoc/>
    public override async Task<ChatMessage> FormatAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        return new ChatMessage(ChatRole.System, await Prompt.FormatAsync(values, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static SystemMessagePromptTemplate FromTemplate(string template)
    {
        return new SystemMessagePromptTemplate(PromptTemplate.FromTemplate(template));
    }
}
