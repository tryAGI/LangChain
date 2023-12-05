namespace LangChain.Schema;

/// <summary>
/// Abstract base class for parsing the outputs of a model.
/// </summary>
public abstract class BaseLlmOutputParser<T>
{
    /// <summary>
    /// Parse a list of candidate model Generations into a specific format.
    /// </summary>
    /// <param name="result">
    ///     A list of Generations to be parsed. The Generations are assumed
    ///     to be different candidate outputs for a single model input.
    /// </param>
    /// <param name="partial"></param>
    /// <returns>Structured output.</returns>
    public abstract Task<T> ParseResult(IReadOnlyList<Generation> result, bool partial = false);
}

/// <summary>
/// Base class to parse the output of an LLM call.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseOutputParser<T> : BaseLlmOutputParser<T>
{
    /// <summary>
    /// Parse a single string model output into some structure.
    /// </summary>
    /// <param name="text">String output of a language model.</param>
    /// <returns></returns>
    public abstract Task<T> Parse(string? text);

    /// <summary>
    /// Parse the output of an LLM call with the input prompt for context.
    /// 
    /// The prompt is largely provided in the event the OutputParser wants
    /// to retry or fix the output in some way, and needs information from
    /// the prompt to do so.
    /// </summary>
    /// <param name="text">String output of a language model.</param>
    /// <param name="prompt">Input PromptValue.</param>
    /// <returns></returns>
    public virtual async Task<T> ParseWithPrompt(string? text, BasePromptValue prompt) => await Parse(text).ConfigureAwait(false);

    /// <summary>
    /// Parse a list of candidate model Generations into a specific format.
    /// <remarks>
    /// The return value is parsed from only the first Generation in the result, which
    /// is assumed to be the highest-likelihood Generation.
    /// </remarks>
    /// </summary>
    /// <param name="result">
    /// A list of Generations to be parsed. The Generations are assumed
    /// to be different candidate outputs for a single model input.
    /// </param>
    /// <param name="partial"></param>
    /// <returns>Structured output.</returns>
    public override Task<T> ParseResult(IReadOnlyList<Generation> result, bool partial = false) => Parse(result[0].Text);

    /// <summary>
    /// Instructions on how the LLM output should be formatted.
    /// </summary>
    /// <returns></returns>
    public virtual string GetFormatInstructions() => throw new NotImplementedException();

    protected virtual string Type() => throw new NotImplementedException("_type not implemented");
}

public class StrOutputParser : BaseOutputParser<string>
{
    public override async Task<string> Parse(string? text) => text;
}