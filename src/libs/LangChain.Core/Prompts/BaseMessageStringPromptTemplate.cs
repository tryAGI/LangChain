using LangChain.Prompts.Base;
using LangChain.Providers;
using LangChain.Schema;

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
    /// <returns></returns>
    public abstract Task<Message> Format(InputValues values);

    /// <inheritdoc/>
    public override async Task<List<Message>> FormatMessages(InputValues values)
    {
        return new List<Message> { await this.Format(values).ConfigureAwait(false) };
    }
}