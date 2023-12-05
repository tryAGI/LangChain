using LangChain.Base;
using LangChain.Cache;
using LangChain.Schema;

namespace LangChain.LLMS;

/// <inheritdoc />
public abstract class BaseLlm : BaseLanguageModel
{
    private readonly BaseCache? _cache;

    /// <inheritdoc />
    protected BaseLlm(IBaseLlmParams parameters) : base(parameters)
    {
        parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        
        _cache = parameters.Cache;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="promptValues"></param>
    /// <param name="stop"></param>
    /// <returns></returns>
    public override async Task<LlmResult> GeneratePrompt(BasePromptValue[] promptValues, IReadOnlyCollection<string>? stop)
    {
        return await Generate(promptValues.Select(p => p.ToString()).ToArray(), stop).ConfigureAwait(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prompts"></param>
    /// <param name="stop"></param>
    /// <returns></returns>
    public abstract Task<LlmResult> Generate(string[] prompts, IReadOnlyCollection<string>? stop);

    /// <summary>
    /// Call the LLM using the provided prompt.
    /// </summary>
    /// <param name="prompt">The prompt to use.</param>
    /// <param name="stop">Whether to stop gathering results.</param>
    /// <returns>A string value containing the LLM response.</returns>
    public async Task<string?> Call(string prompt, List<string>? stop = null)
    {
        var generations = await Generate(new[] { prompt }, stop).ConfigureAwait(false);

        return generations.Generations[0][0].Text;
    }
}