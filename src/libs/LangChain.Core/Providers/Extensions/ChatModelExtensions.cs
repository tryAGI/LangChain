namespace LangChain.Providers;

/// <summary>
///
/// </summary>
public static class ChatModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void EnsureNumbersOfTokensBelowContextLength(
        this IChatModel model,
        int value)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));

        if (value > model.ContextLength)
        {
            throw new InvalidOperationException(
                $"The current number of tokens({value}) is greater than the context length({model.ContextLength}) of the current model.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static void EnsureNumbersOfTokensBelowContextLength(
        this IChatModelWithTokenCounting model,
        string text)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));

        model.EnsureNumbersOfTokensBelowContextLength(model.CountTokens(text));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public static void EnsureNumbersOfTokensBelowContextLength(
        this IChatModelWithTokenCounting model,
        IReadOnlyCollection<Message> messages)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));

        model.EnsureNumbersOfTokensBelowContextLength(model.CountTokens(messages));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static void EnsureNumbersOfTokensBelowContextLength(
        this IChatModelWithTokenCounting model,
        ChatRequest request)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));

        model.EnsureNumbersOfTokensBelowContextLength(model.CountTokens(request));
    }
}