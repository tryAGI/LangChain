using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

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
    public override async Task<Message> FormatAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        return (await Prompt.FormatAsync(values, cancellationToken).ConfigureAwait(false)).AsChatMessage();
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