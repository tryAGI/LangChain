// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public partial class OpenAiChatModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public bool CallFunctionsAutomatically { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public bool ReplyToFunctionCallsAutomatically { get; set; } = true;

    //private List<ChatCompletionFunctions> GlobalFunctions { get; set; } = new();
    private Dictionary<string, Func<string, CancellationToken, Task<string>>> Calls { get; set; } = [];

    #endregion

    #region Methods
    //
    // /// <summary>
    // /// Adds user-defined OpenAI functions to each request to the model.
    // /// </summary>
    // /// <param name="functions"></param>
    // /// <param name="calls"></param>
    // /// <returns></returns>
    // [CLSCompliant(false)]
    // public void AddGlobalFunctions(
    //     ICollection<ChatCompletionFunctions> functions,
    //     IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> calls)
    // {
    //     functions = functions ?? throw new ArgumentNullException(nameof(functions));
    //     calls = calls ?? throw new ArgumentNullException(nameof(calls));
    //
    //     GlobalFunctions.AddRange(functions);
    //     foreach (var call in calls)
    //     {
    //         Calls.Add(call.Key, call.Value);
    //     }
    // }
    //
    // /// <summary>
    // /// 
    // /// </summary>
    // public void ClearGlobalFunctions()
    // {
    //     GlobalFunctions.Clear();
    //     Calls.Clear();
    // }

    #endregion
}