using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve.Abstractions;

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