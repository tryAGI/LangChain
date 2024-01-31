using LangChain.Providers;
using System.Text.Json;

namespace LangChain.Memory;

/// <summary>
/// Stores history in a local file.
/// </summary>
public class FileChatMessageHistory : BaseChatMessageHistory
{
    private string MessagesFilePath { get; }

    private List<Message> _messages;

    /// <inheritdoc/>
    public override IReadOnlyList<Message> Messages
    {
        get
        {
            if (_messages is null)
            {
                LoadMessages().Wait();
            }

            return _messages;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="messagesFilePath">Path to local history file</param>
    /// <exception cref="ArgumentNullException"></exception>
    public FileChatMessageHistory(string messagesFilePath)
    {
        MessagesFilePath = messagesFilePath ?? throw new ArgumentNullException(nameof(messagesFilePath));
    }
        
    /// <inheritdoc/>
    public override async Task AddMessage(Message message)
    {
        _messages.Add(message);
        await SaveMessages();
    }

    /// <inheritdoc/>
    public override async Task Clear()
    {
        _messages.Clear();
        await SaveMessages();
    }

    private async Task SaveMessages()
    {
        string json = JsonSerializer.Serialize(_messages);
        await Task.Run(() => File.WriteAllText(MessagesFilePath, json));
    }

    private async Task LoadMessages()
    {
        if (File.Exists(MessagesFilePath))
        {
            string json = await Task.Run(() => File.ReadAllText(MessagesFilePath));
            _messages = JsonSerializer.Deserialize<List<Message>>(json);
        }
        else
        {
            _messages = new List<Message>();
        }
    }
}