using LangChain.Base;
using LangChain.Schema;

namespace LangChain.Callback;

public class CallbackManagerForToolRun : ParentRunManager
{
    public CallbackManagerForToolRun(string runId, List<BaseCallbackHandler> handlers, List<BaseCallbackHandler> inheritableHandlers, string? parentRunId = null)
        : base(runId, handlers, inheritableHandlers, parentRunId)
    {
    }

    public Task HandleToolErrorAsync(Exception err, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }

    public Task HandleToolEndAsync(string output, string runId, string? parentRunId = null)
    {
        throw new NotImplementedException();
    }
}