using LangChain.Abstractions.Chains.Base;
using LangChain.Sources;
using LangChain.LLMS;
using LangChain.Providers;
using LangChain.Retrievers;
using LangChain.Schema;

namespace LangChain.Base;

/// <inheritdoc />
public abstract class BaseCallbackHandler : IBaseCallbackHandler
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreLlm { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreRetry { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreChain { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreAgent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreRetriever { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IgnoreChatModel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    protected BaseCallbackHandler(IBaseCallbackHandlerInput input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        IgnoreLlm = input.IgnoreLlm;
        IgnoreRetry = input.IgnoreRetry;
        IgnoreChain = input.IgnoreChain;
        IgnoreAgent = input.IgnoreAgent;
        IgnoreRetriever = input.IgnoreRetriever;
        IgnoreChatModel = input.IgnoreChatModel;
    }

    /// <inheritdoc />
    public abstract Task HandleLlmStartAsync(BaseLlm llm, string[] prompts, string runId, string? parentRunId = null,
        IReadOnlyList<string>? tags = null, IReadOnlyDictionary<string, object>? metadata = null,
        string? name = null, IReadOnlyDictionary<string, object>? extraParams = null);

    /// <inheritdoc />
    public abstract Task HandleLlmNewTokenAsync(string token, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleLlmErrorAsync(Exception err, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleLlmEndAsync(LlmResult output, string runId, string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleChatModelStartAsync(
        BaseLlm llm,
        IReadOnlyList<List<Message>> messages,
        string runId,
        string? parentRunId = null,
        IReadOnlyDictionary<string, object>? extraParams = null);

    /// <inheritdoc />
    public abstract Task HandleChainStartAsync(IChain chain, Dictionary<string, object> inputs,
        string runId, string? parentRunId = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null);

    /// <inheritdoc />
    public abstract Task HandleChainErrorAsync(
        Exception err, string runId,
        Dictionary<string, object>? inputs = null,
        string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleChainEndAsync(
        Dictionary<string, object>? inputs,
        Dictionary<string, object> outputs,
        string runId,
        string? parentRunId = null);

    /// <inheritdoc />
    public abstract Task HandleToolStartAsync(
        Dictionary<string, object> tool,
        string input, string runId,
        string? parentRunId = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null);

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
    public abstract Task HandleRetrieverStartAsync(
        BaseRetriever retriever,
        string query,
        string runId,
        string? parentRunId,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null);

    /// <inheritdoc />
    public abstract Task HandleRetrieverEndAsync(
        string query,
        List<Document> documents,
        string runId,
        string? parentRunId);

    /// <inheritdoc />
    public abstract Task HandleRetrieverErrorAsync(Exception exception, string query, string runId, string? parentRunId);
}