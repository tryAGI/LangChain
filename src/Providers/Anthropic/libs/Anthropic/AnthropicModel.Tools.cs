using LangChain.Providers.Anthropic.Tools;

namespace LangChain.Providers.Anthropic;

public partial class AnthropicModel
{
    private string? GetSystemMessage()
    {
        if (GlobalTools != null && GlobalTools.Count > 0)
        {
            var tools = new AnthropicTools();
            tools.Tools = GlobalTools;
            var toolsString = tools.ToXml();

            return
                $"In this environment you have access to a set of tools you can use to answer the user's question. \r\n\r\nYou may call them like this:\r\n<function_calls>\r\n<invoke>\r\n<tool_name>$TOOL_NAME</tool_name>\r\n<parameters>\r\n<$PARAMETER_NAME>$PARAMETER_VALUE</$PARAMETER_NAME>\r\n...\r\n</parameters>\r\n</invoke>\r\n</function_calls>\r\n\r\n{toolsString}\r\n\r\nIf you already know the answer then simply reply normally, otherwise call the function when necessary.";
        }

        return null;
    }

    #region Properties

    /// <summary>
    /// </summary>
    public bool CallToolsAutomatically { get; set; } = true;

    /// <summary>
    /// </summary>
    public bool ReplyToToolCallsAutomatically { get; set; } = true;

    protected List<AnthropicTool> GlobalTools { get; set; } = [];
    protected Dictionary<string, Func<string, CancellationToken, Task<string>>> Calls { get; set; } = [];

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
        ICollection<AnthropicTool> tools,
        IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> calls)
    {
        tools = tools ?? throw new ArgumentNullException(nameof(tools));
        calls = calls ?? throw new ArgumentNullException(nameof(calls));

        GlobalTools.AddRange(tools);
        foreach (var call in calls) Calls.Add(call.Key, call.Value);
    }

    /// <summary>
    /// </summary>
    public void ClearGlobalTools()
    {
        GlobalTools.Clear();
        Calls.Clear();
    }

    #endregion
}