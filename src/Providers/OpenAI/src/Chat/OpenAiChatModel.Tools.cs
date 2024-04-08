using OpenAI;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public partial class OpenAiChatModel
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

    protected List<Tool> GlobalTools { get; set; } = [];
    protected Dictionary<string, Func<string, CancellationToken, Task<string>>> Calls { get; set; } = [];

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
        ICollection<Tool> tools,
        IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> calls)
    {
        tools = tools ?? throw new ArgumentNullException(nameof(tools));
        calls = calls ?? throw new ArgumentNullException(nameof(calls));
    
        GlobalTools.AddRange(tools);
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