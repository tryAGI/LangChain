using LangChain.Serve.Classes.Repository;

namespace LangChain.Serve.Interfaces;

public interface IConversationRepository
{
    public Task<StoredConversation> CreateConversation(string modelName);
    public Task UpdateConversationName(Guid conversationId, string conversationName);
    public Task<StoredConversation?> GetConversation(Guid conversationId);
    public Task DeleteConversation(Guid conversationId);
    public Task<List<StoredConversation>> ListConversations();

    public Task AddMessage(StoredMessage message);
    public Task<List<StoredMessage>> ListMessages(Guid conversationId);
}