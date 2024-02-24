using LangChain.Schema;

namespace LangChain.Prompts.Base;

/// <inheritdoc />
public abstract class BaseStringPromptTemplate : BasePromptTemplate
{
    /// <inheritdoc />
    public override async Task<BasePromptValue> FormatPromptValue(InputValues values)
    {
        var formattedPrompt = await Format(values).ConfigureAwait(false);

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