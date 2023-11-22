using LangChain.Abstractions.Chains.Base;
using LangChain.Docstore;
using LangChain.LLMS;
using LangChain.Providers;
using LangChain.Retrievers;
using LangChain.Schema;

namespace LangChain.Base.Tracers;

/// <summary>
/// Base class for tracers.
/// </summary>
public abstract class BaseTracer(IBaseCallbackHandlerInput input) : BaseCallbackHandler(input)
{
    protected Dictionary<string, Run> RunMap { get; } = new();

    protected abstract Task PersistRun(Run run);

    public override async Task HandleLlmStartAsync(
        BaseLlm llm,
        string[] prompts,
        string runId,
        string? parentRunId = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string name = null,
        Dictionary<string, object>? extraParams = null)
    {
        var executionOrder = GetExecutionOrder(parentRunId);
        var startTime = DateTime.UtcNow;
        if (metadata != null)
        {
            extraParams.Add("metadata", metadata);
        }

        var run = new Run
        {
            Id = runId,
            ParentRunId = parentRunId,
            //todo: pass llm or dumpd(llm)
            // serialized = serialized,
            Inputs = new Dictionary<string, object> { ["prompts"] = prompts },
            ExtraData = extraParams,
            Events = new List<Dictionary<string, object>>
            {
                new()
                {
                    ["name"] = "start",
                    ["time"] = startTime
                }
            },
            StartTime = startTime,
            ExecutionOrder = executionOrder,
            ChildExecutionOrder = executionOrder,
            RunType = "llm",
            Tags = tags ?? new List<string>(),
            Name = name
        };

        StartTrace(run);
        await HandleLlmStartAsync(run);
    }

    public override async Task HandleLlmErrorAsync(Exception err, string runId, string? parentRunId = null)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_llm_error callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run) || run.RunType != "llm")
        {
            throw new TracerException($"No LLM Run found to be traced for {runId}");
        }

        run.Error = err.ToString();
        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object> { ["name"] = "error", ["time"] = run.EndTime });

        EndTrace(run);
        await HandleLlmErrorAsync(run);
    }

    public override async Task HandleLlmEndAsync(LlmResult output, string runId, string? parentRunId = null)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_llm_end callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run) || run.RunType != "llm")
        {
            throw new TracerException($"No LLM Run found to be traced for {runId}");
        }

        run.Outputs = output.LlmOutput;
        for (var i = 0; i < output.Generations.Length; i++)
        {
            for (var j = 0; j < output.Generations[i].Length; j++)
            {
                var generation = output.Generations[i][j];

                var outputGeneration = (run.Outputs["generations"] as List<Dictionary<string, string>>)[i];
                if (outputGeneration.ContainsKey("message"))
                {
                    outputGeneration["message"] = (generation as ChatGeneration)?.Message;
                }
            }
        }

        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object> { { "name", "end" }, { "time", run.EndTime } });

        EndTrace(run);
        await HandleLlmEndAsync(run);
    }

    public override async Task HandleLlmNewTokenAsync(string token, string runId, string? parentRunId = null)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_llm_new_token callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run) || run.RunType != "llm")
        {
            throw new TracerException($"No LLM Run found to be traced for {runId}");
        }

        var eventData = new Dictionary<string, object> { ["token"] = token };

        run.Events.Add(
            new()
            {
                ["name"] = "new_token",
                ["time"] = DateTime.UtcNow,
                ["kwargs"] = eventData,
            });

        await HandleLlmNewTokenAsync(run, token);
    }

    public override async Task HandleChatModelStartAsync(BaseLlm llm, List<List<Message>> messages, string runId,
        string? parentRunId = null,
        Dictionary<string, object>? extraParams = null)
    {
        throw new NotImplementedException();
    }

    public override async Task HandleChainStartAsync(
        IChain chain,
        Dictionary<string, object> inputs,
        string runId,
        string? parentRunId = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null)
    {
        var executionOrder = GetExecutionOrder(parentRunId);
        var startTime = DateTime.UtcNow;

        if (metadata != null)
        {
            extraParams.Add("metadata", metadata);
        }

        var chainRun = new Run
        {
            Id = runId,
            ParentRunId = parentRunId,
            // serialized=serialized,
            Inputs = inputs,
            ExtraData = extraParams,
            Events = new List<Dictionary<string, object>> { new() { ["name"] = "start", ["time"] = startTime } },
            StartTime = startTime,
            ExecutionOrder = executionOrder,
            ChildExecutionOrder = executionOrder,
            ChildRuns = new(),
            RunType = runType ?? "chain",
            Name = name,
            Tags = tags ?? new()
        };

        StartTrace(chainRun);
        await HandleChainStartAsync(chainRun);
    }

    /// <summary>
    /// Handle an error for a chain run.
    /// </summary>
    public override async Task HandleChainErrorAsync(
        Exception err,
        string runId,
        Dictionary<string, object>? inputs = null,
        string? parentRunId = null)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_chain_error callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run))
        {
            throw new TracerException($"No chain Run found to be traced for {runId}");
        }

        run.Error = err.ToString();
        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object> { ["name"] = "error", ["time"] = run.EndTime });

        run.Inputs = inputs;
        EndTrace(run);
        await HandleChainErrorAsync(run);
    }

    /// <summary>
    /// End a trace for a chain run.
    /// </summary>
    public override async Task HandleChainEndAsync(
        Dictionary<string, object>? inputs,
        Dictionary<string, object> outputs,
        string runId,
        string? parentRunId = null)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_chain_end callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run))
        {
            throw new TracerException($"No chain Run found to be traced for {runId}");
        }

        run.Outputs = outputs;
        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object> { ["name"] = "end", ["time"] = run.EndTime });

        run.Inputs = inputs;

        EndTrace(run);
        await HandleChainEndAsync(run);
    }

    public override async Task HandleToolStartAsync(
        Dictionary<string, object> tool,
        string input,
        string runId,
        string? parentRunId = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string runType = null,
        string name = null,
        Dictionary<string, object>? extraParams = null)
    {
        var executionOrder = GetExecutionOrder(parentRunId);
        var startTime = DateTime.UtcNow;

        if (metadata != null)
        { extraParams.Add("metadata", metadata); }

        var run = new Run
        {
            Id = runId,
            ParentRunId = parentRunId,
            Serialized = tool,
            Inputs = new Dictionary<string, object> { ["input"] = input },
            ExtraData = extraParams,
            Events = new List<Dictionary<string, object>> { new() { ["name"] = "start", ["time"] = startTime } },
            StartTime = startTime,
            ExecutionOrder = executionOrder,
            ChildExecutionOrder = executionOrder,
            ChildRuns = new(),
            RunType = "tool",
            Tags = tags ?? new(),
            Name = name,
        };

        StartTrace(run);
        await HandleToolStartAsync(run);
    }

    /// <summary>
    /// Handle an error for a tool run.
    /// </summary>
    public override async Task HandleToolErrorAsync(Exception err, string runId, string? parentRunId = null)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_tool_error callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run) || run.RunType != "tool")
        {
            throw new TracerException($"No retriever Run found to be traced for {runId}");
        }

        run.Error = err.ToString();
        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object> { ["name"] = "error", ["time"] = run.EndTime });
        EndTrace(run);
        await HandleToolErrorAsync(run);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="output"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <exception cref="TracerException"></exception>
    public override async Task HandleToolEndAsync(string output, string runId, string? parentRunId = null)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_tool_end callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run) || run.RunType != "tool")
        {
            throw new TracerException($"No retriever Run found to be traced for {runId}");
        }

        run.Outputs = new Dictionary<string, object>()
        {
            ["output"] = output
        };
        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object> { ["name"] = "end", ["time"] = run.EndTime });
        EndTrace(run);
        await HandleToolEndAsync(run);
    }

    /// <summary>
    /// Run when Retriever starts running.
    /// </summary>
    public override async Task HandleRetrieverStartAsync(
        BaseRetriever retriever,
        string query,
        string runId,
        string? parentRunId,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null,
        string? runType = null,
        string? name = null,
        Dictionary<string, object>? extraParams = null)
    {
        var executionOrder = GetExecutionOrder(parentRunId);
        var startTime = DateTime.UtcNow;

        if (metadata != null)
        {
            extraParams.Add("metadata", metadata);
        }

        var run = new Run
        {
            Id = runId,
            Name = name ?? "Retriever",
            ParentRunId = parentRunId,
            // TODO: pass retriever or dumpd(retriever)?
            // serialized=serialized,
            Inputs = new Dictionary<string, object> { ["query"] = query },
            ExtraData = extraParams,
            Events = new List<Dictionary<string, object>> { new() { ["name"] = "start", ["time"] = startTime } },
            StartTime = startTime,
            ExecutionOrder = executionOrder,
            ChildExecutionOrder = executionOrder,
            Tags = tags,
            ChildRuns = new(),
            RunType = "retriever",
        };

        StartTrace(run);
        await HandleRetrieverStartAsync(run);
    }

    /// <summary>
    /// Run when Retriever ends running.
    /// </summary>
    public override async Task HandleRetrieverEndAsync(
        string query,
        List<Document> documents,
        string runId,
        string? parentRunId)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_retriever_end callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run) || run.RunType != "retriever")
        {
            throw new TracerException($"No retriever Run found to be traced for {runId}");
        }

        run.Outputs = new Dictionary<string, object> { ["documents"] = documents };
        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object>
        { ["name"] = "end", ["time"] = run.EndTime });

        EndTrace(run);
        await HandleRetrieverEndAsync(run);
    }

    /// <summary>
    /// Run when Retriever errors.
    /// </summary>
    public override async Task HandleRetrieverErrorAsync(Exception error, string query, string runId,
        string? parentRunId)
    {
        if (runId == null)
        {
            throw new TracerException("No run_id provided for on_retriever_end callback.");
        }

        if (!RunMap.TryGetValue(runId, out var run) || run.RunType != "retriever")
        {
            throw new TracerException($"No retriever Run found to be traced for {runId}");
        }

        run.Error = error.ToString();
        run.EndTime = DateTime.UtcNow;
        run.Events.Add(new Dictionary<string, object>
        {
            ["name"] = "error",
            ["time"] = run.EndTime
        });

        EndTrace(run);
        await HandleRetrieverErrorAsync(run);
    }

    public override async Task HandleTextAsync(string text, string runId, string? parentRunId = null)
    {
    }

    public override async Task HandleAgentActionAsync(Dictionary<string, object> action, string runId, string? parentRunId = null)
    {
    }

    public override async Task HandleAgentEndAsync(Dictionary<string, object> action, string runId, string? parentRunId = null)
    {
    }

    /*public Run OnRetry(RetryCallState retryState, string runId)
   {
       if (runId == null)
       {
           throw new TracerException("No run_id provided for on_retry callback.");
       }

       if (!_runMap.TryGetValue(runId, out var run) || run == null)
       {
           throw new TracerException("No Run found to be traced for on_retry");
       }

       var kwargs = new Dictionary<string, object>
       {
           { "slept", retryState.IdleFor },
           { "attempt", retryState.AttemptNumber }
       };

       if (retryState.Outcome == null)
       {
           kwargs["outcome"] = "N/A";
       }
       else if (retryState.Outcome.Failed)
       {
           kwargs["outcome"] = "failed";
           Exception exception = retryState.Outcome.Exception();
           kwargs["exception"] = exception.ToString();
           kwargs["exception_type"] = exception.GetType().Name;
       }
       else
       {
           kwargs["outcome"] = "success";
           kwargs["result"] = retryState.Outcome.Result().ToString();
       }

       run.events.Add(new Dictionary<string, object>
       {
           { "name", "retry" },
           { "time", DateTime.UtcNow },
           { "kwargs", kwargs }
       });

       return run;
   }*/

    /// <summary>
    /// Process a run upon creation.
    /// </summary>
    protected abstract void OnRunCreate(Run run);

    /// <summary>
    /// Process a run upon update.
    /// </summary>
    protected abstract void OnRunUpdate(Run run);

    protected abstract Task HandleLlmStartAsync(Run run);
    protected abstract Task HandleLlmNewTokenAsync(Run run, string token);
    protected abstract Task HandleLlmErrorAsync(Run run);
    protected abstract Task HandleLlmEndAsync(Run run);
    protected abstract Task HandleChatModelStartAsync(Run run);
    protected abstract Task HandleChainStartAsync(Run run);
    protected abstract Task HandleChainErrorAsync(Run run);
    protected abstract Task HandleChainEndAsync(Run run);
    protected abstract Task HandleToolStartAsync(Run run);
    protected abstract Task HandleToolErrorAsync(Run run);
    protected abstract Task HandleToolEndAsync(Run run);
    protected abstract Task HandleTextAsync(Run run);
    protected abstract Task HandleAgentActionAsync(Run run);
    protected abstract Task HandleAgentEndAsync(Run run);
    protected abstract Task HandleRetrieverStartAsync(Run run);
    protected abstract Task HandleRetrieverEndAsync(Run run);
    protected abstract Task HandleRetrieverErrorAsync(Run run);

    /// <summary>
    /// Add child run to a chain run or tool run.
    /// </summary>
    /// <param name="parentRun"></param>
    /// <param name="childRun"></param>
    private static void AddChildRun(Run parentRun, Run childRun) => parentRun.ChildRuns.Add(childRun);

    //Start a trace for a run.
    private void StartTrace(Run run)
    {
        if (run.ParentRunId != null)
        {
            if (RunMap.TryGetValue(run.ParentRunId, out var parentRun))
            {
                AddChildRun(parentRun, run);

                parentRun.ChildExecutionOrder =
                    Math.Max(parentRun.ChildExecutionOrder ?? 0, run.ChildExecutionOrder ?? 0);
            }
            else
            {
                Console.WriteLine($"Parent run with id {run.ParentRunId} not found.");
            }
        }

        RunMap[run.Id] = run;
        OnRunCreate(run);
    }

    //End a trace for a run.
    private void EndTrace(Run run)
    {
        if (run.ParentRunId == null)
        {
            PersistRun(run);
        }
        else
        {
            if (RunMap.TryGetValue(run.ParentRunId, out var parentRun))
            {
                if (run.ChildExecutionOrder != null && parentRun.ChildExecutionOrder != null &&
                    run.ChildExecutionOrder > parentRun.ChildExecutionOrder)
                {
                    parentRun.ChildExecutionOrder = run.ChildExecutionOrder;
                }
            }
            else
            {
                Console.WriteLine($"Parent run with id {run.ParentRunId} not found.");
            }
        }

        RunMap.Remove(run.Id);
        OnRunUpdate(run);
    }

    //Get the execution order for a run.
    private int GetExecutionOrder(string? parentRunId = null)
    {
        if (parentRunId == null)
        {
            return 1;
        }

        if (RunMap.TryGetValue(parentRunId, out var parentRun))
        {
            if (parentRun.ChildExecutionOrder == null)
            {
                throw new TracerException($"Parent run with id {parentRunId} has no child execution order.");
            }
        }
        else
        {
            Console.WriteLine($"Parent run with id {parentRunId} not found.");
        }

        return 1;
    }
}