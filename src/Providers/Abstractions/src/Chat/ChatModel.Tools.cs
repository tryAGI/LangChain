// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public partial class ChatModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public bool CallToolsAutomatically { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public bool ReplyToToolCallsAutomatically { get; set; } = true;

    [CLSCompliant(false)]
    protected IList<OpenApiSchema> GlobalTools { get; } = [];
    protected Dictionary<string, Func<string, CancellationToken, Task<string>>> Calls { get; } = [];

    #endregion

    #region Methods

    /// <summary>
    /// Adds user-defined OpenAI tools to each request to the model.
    /// </summary>
    /// <param name="tools"></param>
    /// <param name="calls"></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public void AddGlobalTools(
        ICollection<OpenApiSchema> tools,
        IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> calls)
    {
        tools = tools ?? throw new ArgumentNullException(nameof(tools));
        calls = calls ?? throw new ArgumentNullException(nameof(calls));

        foreach (var tool in tools)
        {
            GlobalTools.Add(tool);
        }
        foreach (var call in calls)
        {
            Calls.Add(call.Key, call.Value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ClearGlobalTools()
    {
        GlobalTools.Clear();
        Calls.Clear();
    }

    #endregion
}