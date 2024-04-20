using LangChain.Providers;

namespace LangChain.Memory;

/// <summary>
/// Abstract base class for storing chat message history.
/// 
/// Implementations should over-ride the AddMessages method to handle bulk addition
/// of messages.
///     
/// The default implementation of AddMessages will correctly call AddMessage, so
/// it is not necessary to implement both methods.
///     
/// When used for updating history, users should favor usage of `AddMessages`
/// over `AddMessage` or other variants like `AddUserMessage` and `AddAiMessage`
/// to avoid unnecessary round-trips to the underlying persistence layer.
/// </summary>
public abstract class BaseChatMessageHistory
{
    /// <summary>
    /// A list of messages stored in-memory.
    /// </summary>
    public abstract IReadOnlyList<Message> Messages { get; }

    /// <summary>
    /// Convenience method for adding a human message string to the store.
    /// 
    /// Please note that this is a convenience method.  Code should favor the
    /// bulk AddMessages interface instead to save on round-trips to the underlying
    /// persistence layer.
    /// 
    /// This method may be deprecated in a future release.
    /// </summary>
    /// <param name="message">The human message to add</param>
    public async Task AddUserMessage(string message)
    {
        await AddMessage(message.AsHumanMessage()).ConfigureAwait(false);
    }

    /// <summary>
    /// Convenience method for adding an AI message string to the store.
    /// 
    /// Please note that this is a convenience method. Code should favor the bulk
    /// AddMessages interface instead to save on round-trips to the underlying
    /// persistence layer.
    /// 
    /// This method may be deprecated in a future release.
    /// </summary>
    /// <param name="message"></param>
    public async Task AddAiMessage(string message)
    {
        await AddMessage(message.AsAiMessage()).ConfigureAwait(false);
    }

    /// <summary>
    /// Add a message object to the store.
    /// </summary>
    /// <param name="message">A message object to store</param>
    public abstract Task AddMessage(Message message);

    /// <summary>
    /// Add a list of messages.
    /// 
    /// Implementations should override this method to handle bulk addition of messages
    /// in an efficient manner to avoid unnecessary round-trips to the underlying store.
    /// </summary>
    /// <param name="messages">A list of message objects to store.</param>
    public virtual async Task AddMessages(IEnumerable<Message> messages)
    {
        messages = messages ?? throw new ArgumentNullException(nameof(messages));

        foreach (var message in messages)
        {
            await AddMessage(message).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Replace the list of messages.
    /// 
    /// Implementations should override this method to handle bulk addition of messages
    /// in an efficient manner to avoid unnecessary round-trips to the underlying store.
    /// </summary>
    /// <param name="messages">A list of message objects to store.</param>
    public virtual async Task SetMessages(IEnumerable<Message> messages)
    {
        messages = messages ?? throw new ArgumentNullException(nameof(messages));

        await Clear().ConfigureAwait(false);
        await AddMessages(messages).ConfigureAwait(false);
    }

    /// <summary>
    /// Remove all messages from the store
    /// </summary>
    public abstract Task Clear();
}