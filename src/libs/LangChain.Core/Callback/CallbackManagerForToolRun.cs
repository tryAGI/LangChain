using LangChain.Base;
using LangChain.Schema;

namespace LangChain.Callback;

/// <inheritdoc/>
public class CallbackManagerForToolRun : ParentRunManager
{
    /// <inheritdoc/>
    public CallbackManagerForToolRun(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="err"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task HandleToolErrorAsync(Exception err, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="output"></param>
    /// <param name="runId"></param>
    /// <param name="parentRunId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task HandleToolEndAsync(string output, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }
}