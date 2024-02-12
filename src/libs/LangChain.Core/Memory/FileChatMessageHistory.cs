using LangChain.Providers;
using System.Text.Json;

namespace LangChain.Memory;

/// <summary>
/// Chat message history that stores history in a local file.
/// </summary>
public class FileChatMessageHistory : BaseChatMessageHistory
{
    private string MessagesFilePath { get; }

    private List<Message> _messages = new List<Message>();

    /// <inheritdoc/>
    public override IReadOnlyList<Message> Messages => _messages;

    /// <summary>
    /// Initializes new history instance with provided file path
    /// </summary>
    /// <param name="messagesFilePath">path of the local file to store the messages</param>
    /// <exception cref="ArgumentNullException"></exception>
    private FileChatMessageHistory(string messagesFilePath)
    {
        MessagesFilePath = messagesFilePath ?? throw new ArgumentNullException(nameof(messagesFilePath));
    }

    /// <summary>
    /// Create new history instance with provided file path
    /// </summary>
    /// <param name="path">path of the local file to store the messages</param>
    /// <param name="cancellationToken"></param>
    public static async Task<FileChatMessageHistory> CreateAsync(string path, CancellationToken cancellationToken = default)
    {
        FileChatMessageHistory chatHistory = new FileChatMessageHistory(path);
        await chatHistory.LoadMessages().ConfigureAwait(false);

        return chatHistory;
    }

    /// <inheritdoc/>
    public override async Task AddMessage(Message message)
    {
        _messages.Add(message);
        await SaveMessages().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task Clear()
    {
        _messages.Clear();
        await SaveMessages().ConfigureAwait(false);
    }

    private async Task SaveMessages()
    {
        string json = JsonSerializer.Serialize(_messages);
        await Task.Run(() => File.WriteAllText(MessagesFilePath, json)).ConfigureAwait(false);
    }

    private Task LoadMessages()
    {
        try
        {
            if (File.Exists(MessagesFilePath))
            {
                string json = File.ReadAllText(MessagesFilePath);
                _messages = JsonSerializer.Deserialize<List<Message>>(json) ?? new List<Message>();
            }
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
}