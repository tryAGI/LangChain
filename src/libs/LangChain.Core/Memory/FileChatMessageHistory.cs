﻿using LangChain.Providers;
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
    public FileChatMessageHistory(string messagesFilePath)
    {
        MessagesFilePath = messagesFilePath ?? throw new ArgumentNullException(nameof(messagesFilePath));

        // Blocking call in the constructor creates a simpler implementation
        LoadMessages().Wait();
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
        await Task.Run(() => File.WriteAllText(MessagesFilePath, json)).ConfigureAwait(false);
    }

    private async Task LoadMessages()
    {
        if (File.Exists(MessagesFilePath))
        {
            string json = await Task.Run(() => File.ReadAllText(MessagesFilePath)).ConfigureAwait(false);
            _messages = JsonSerializer.Deserialize<List<Message>>(json);
        }
    }

}