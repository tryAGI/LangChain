using LangChain.Schema;

namespace LangChain.Prompts.Base;

/// <inheritdoc />
public abstract class BaseStringPromptTemplate : BasePromptTemplate
{
    /// <inheritdoc />
    public override async Task<BasePromptValue> FormatPromptValueAsync(
        InputValues values,
        CancellationToken cancellationToken = default)
    {
        var formattedPrompt = await FormatAsync(values, cancellationToken).ConfigureAwait(false);

        return new StringPromptValue()
        {
            Value = formattedPrompt
        };
    }

    /// <inheritdoc />
    protected BaseStringPromptTemplate(IBasePromptTemplateInput input) : base(input)
    {
    }
}