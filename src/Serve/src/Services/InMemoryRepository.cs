using LangChain.Serve.Interfaces;
using LangChain.Serve.Classes.Repository;

namespace LangChain.Serve.Services;

public class InMemoryRepository : IConversationRepository
{
    private readonly List<StoredConversation> _conversations = [];
    private readonly List<StoredMessage> _messages = [];

    public Task<StoredConversation> CreateConversation(string modelName)
    {
        var conversation = new StoredConversation
        {
            ConversationId = Guid.NewGuid(),
            ModelName = modelName,
            ConversationName = null,
            CreatedAt = DateTime.UtcNow
        };

        _conversations.Insert(0,conversation);
        return Task.FromResult(conversation);
    }

    public Task UpdateConversationName(Guid conversationId, string conversationName)
    {
        var conversation = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        if (conversation != null)
        {
            conversation.ConversationName = conversationName;
        }
        return Task.CompletedTask;
    }

    public Task<StoredConversation?> GetConversation(Guid conversationId)
    {
        var conversation = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        return Task.FromResult(conversation);
    }

    public Task DeleteConversation(Guid conversationId)
    {
        var conversation = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        if (conversation != null)
        {
            _conversations.Remove(conversation);
            _messages.RemoveAll(m => m.ConversationId == conversationId);
        }

        return Task.CompletedTask;
    }

    public Task<List<StoredConversation>> ListConversations()
    {
        return Task.FromResult(_conversations);
    }

    public Task AddMessage(StoredMessage message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public Task<List<StoredMessage>> ListMessages(Guid conversationId)
    {
        var result = _messages.Where(m => m.ConversationId == conversationId).ToList();
        return Task.FromResult(result);
    }
}
