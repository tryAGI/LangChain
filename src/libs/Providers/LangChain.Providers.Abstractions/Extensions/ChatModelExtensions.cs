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
    /// <param name="prompt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string> GenerateAsync(
        this IChatModel model,
        string prompt,
        CancellationToken cancellationToken = default)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));
        
        var response = await model.GenerateAsync(
            request: new ChatRequest(Messages: new[]
            {
                prompt.AsHumanMessage(),
            }),
            cancellationToken).ConfigureAwait(false);

        return response.Messages.First().Content;
    }

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
                $"The current number of tokens({value}) is greater than the context length({model.ContextLength}) of the current model({model.Id}).");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static void EnsureNumbersOfTokensBelowContextLength(
        this IChatModel model,
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
        this IChatModel model,
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
        this IChatModel model,
        ChatRequest request)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));

        model.EnsureNumbersOfTokensBelowContextLength(model.CountTokens(request));
    }
}