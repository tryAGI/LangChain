using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

/// <inheritdoc/>
public class AiMessagePromptTemplate : BaseMessageStringPromptTemplate
{
    /// <inheritdoc/>
    public AiMessagePromptTemplate(BaseStringPromptTemplate prompt) : base(prompt) { }

    /// <inheritdoc/>
    public override async Task<Message> FormatAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        return (await Prompt.FormatAsync(values, cancellationToken).ConfigureAwait(false)).AsAiMessage();
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