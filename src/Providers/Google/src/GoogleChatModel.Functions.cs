using GenerativeAI.Tools;

namespace LangChain.Providers.Google;

public partial class GoogleChatModel
{
    #region Properties

    /// <summary>
    /// </summary>
    public bool CallToolsAutomatically { get; set; } = true;

    /// <summary>
    /// </summary>
    public bool ReplyToToolCallsAutomatically { get; set; } = true;

    private List<ChatCompletionFunction> GlobalFunctions { get; } = [];
    private Dictionary<string, Func<string, CancellationToken, Task<string>>> Calls { get; } = [];

    #endregion

    #region Methods

    /// <summary>
    ///     Adds user-defined OpenAI tools to each request to the model.
    /// </summary>
    /// <param name="tools"></param>
    /// <param name="calls"></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public void AddGlobalTools(
        ICollection<ChatCompletionFunction> tools,
        IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> calls)
    {
        tools = tools ?? throw new ArgumentNullException(nameof(tools));
        calls = calls ?? throw new ArgumentNullException(nameof(calls));

        GlobalFunctions.AddRange(tools);
        foreach (var call in calls) Calls.Add(call.Key, call.Value);
    }

    /// <summary>
    /// </summary>
    public void ClearGlobalTools()
    {
        GlobalFunctions.Clear();
        Calls.Clear();
    }

    #endregion
}