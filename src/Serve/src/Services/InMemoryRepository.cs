using LangChain.Serve.Interfaces;
using LangChain.Utilities.Classes.Repository;

namespace LangChain.Utilities.Services;

public class InMemoryRepository : IConversationRepository
{
    private readonly List<StoredConversation> conversations = new List<StoredConversation>();
    private readonly List<StoredMessage> messages = new List<StoredMessage>();

    public Task<StoredConversation> CreateConversation(string modelName)
    {
        var conversation = new StoredConversation
        {
            ConversationId = Guid.NewGuid(),
            ModelName = modelName,
            ConversationName = null,
            CreatedAt = DateTime.UtcNow
        };

        conversations.Insert(0,conversation);
        return Task.FromResult(conversation);
    }

    public Task UpdateConversationName(Guid conversationId, string conversationName)
    {
        var conversation = conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        if (conversation != null)
        {
            conversation.ConversationName = conversationName;
        }
        return Task.CompletedTask;
    }

    public Task<StoredConversation?> GetConversation(Guid conversationId)
    {
        var conversation = conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        return Task.FromResult(conversation);
    }

    public Task DeleteConversation(Guid conversationId)
    {
        var conversation = conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        if (conversation != null)
        {
            conversations.Remove(conversation);
            messages.RemoveAll(m => m.ConversationId == conversationId);
        }

        return Task.CompletedTask;
    }

    public Task<List<StoredConversation>> ListConversations()
    {
        return Task.FromResult(conversations);
    }

    public Task AddMessage(StoredMessage message)
    {
        messages.Add(message);
        return Task.CompletedTask;
    }

    public Task<List<StoredMessage>> ListMessages(Guid conversationId)
    {
        var result = messages.Where(m => m.ConversationId == conversationId).ToList();
        return Task.FromResult(result);
    }
}
