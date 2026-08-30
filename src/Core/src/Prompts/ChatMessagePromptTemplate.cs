using LangChain.Prompts.Base;
using LangChain.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class ChatMessagePromptTemplate(
    BaseStringPromptTemplate prompt,
    string role)
    : BaseMessageStringPromptTemplate(prompt)
{
    /// <summary>
    ///
    /// </summary>
    public string Role { get; set; } = role;

    /// <inheritdoc/>
    public override async Task<ChatMessage> FormatAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        return new ChatMessage(new ChatRole(Role), await Prompt.FormatAsync(values, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="template"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public static ChatMessagePromptTemplate FromTemplate(string template, string role)
    {
        return new ChatMessagePromptTemplate(PromptTemplate.FromTemplate(template), role);
    }
}
