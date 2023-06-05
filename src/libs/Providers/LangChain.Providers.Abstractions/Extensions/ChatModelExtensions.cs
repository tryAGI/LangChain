namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public static class ChatModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Message AsSystemMessage(this string text)
    {
        return new Message(text, MessageRole.System);
    }

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
}