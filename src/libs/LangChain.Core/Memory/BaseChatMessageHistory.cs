using LangChain.Providers;

namespace LangChain.Memory;

/// <summary>
/// 
/// </summary>
public abstract class BaseChatMessageHistory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public async Task AddUserMessage(string message)
    {
        await AddMessage(message.AsHumanMessage()).ConfigureAwait(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public async Task AddAiMessage(string message)
    {
        await AddMessage(message.AsAiMessage()).ConfigureAwait(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract IReadOnlyList<Message> Messages { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public abstract Task AddMessage(Message message);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract Task Clear();
}