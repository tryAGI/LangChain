using LangChain.LLMS;
using LangChain.Schema;

namespace LangChain.Base;

/// <inheritdoc />
public abstract class BaseCallbackHandler : IBaseCallbackHandler
{
    /// <inheritdoc />
    public string Name { get; protected set; }

    /// <inheritdoc />
    public abstract Task HandleLlmStartAsync(BaseLlm llm, string[] prompts, string runId, string? parentRunId = null,
        Dictionary<string, object>? extraParams = null);

    /// <inheritdoc />
    public abstract Task HandleLlmNewTokenAsync(string token, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleLlmErrorAsync(Exception err, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleLlmEndAsync(LlmResult output, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleChatModelStartAsync(Dictionary<string, object> llm, List<List<object>> messages, string runId, string? parentRunId = null,
        Dictionary<string, object>? extraParams = null);

    /// <inheritdoc />
    public abstract Task HandleChainStartAsync(Dictionary<string, object> chain, Dictionary<string, object> inputs, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleChainErrorAsync(Exception err, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleChainEndAsync(Dictionary<string, object> outputs, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleToolStartAsync(Dictionary<string, object> tool, string input, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleToolErrorAsync(Exception err, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleToolEndAsync(string output, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleTextAsync(string text, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleAgentActionAsync(Dictionary<string, object> action, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleAgentEndAsync(Dictionary<string, object> action, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleRetrieverStartAsync(string query, string runId, string? parentRunId);

    /// <inheritdoc />
    public abstract Task HandleRetrieverEndAsync(string query, string runId, string? parentRunId);

    /// <inheritdoc />
    public abstract Task HandleRetrieverErrorAsync(Exception error, string query, string runId, string? parentRunId);

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreLlm { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreChain { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreAgent { get; set; }

    public bool IgnoreRetriever { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected BaseCallbackHandler()
    {
        Name = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    protected BaseCallbackHandler(IBaseCallbackHandlerInput input) : this()
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        IgnoreLlm = input.IgnoreLlm;
        IgnoreChain = input.IgnoreChain;
        IgnoreAgent = input.IgnoreAgent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract IBaseCallbackHandler Copy();
}