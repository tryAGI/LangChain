using LangChain.Utilities.Classes.Repository;

namespace LangChain.Serve.Classes.DTO;

public class ConversationDTO
{
    public Guid ConversationId { get; set; }
    public string ModelName { get; set; }
    public string? ConversationName { get; set; }
    public DateTime CreatedAt { get; set; }

    public static ConversationDTO FromStoredConversation(StoredConversation conversation)
    {
        if (conversation == null)
        {
            return null;
        }
        return new ConversationDTO
        {
            ConversationId = conversation.ConversationId,
            ModelName = conversation.ModelName,
            ConversationName = conversation.ConversationName,
            CreatedAt = conversation.CreatedAt
        };
    }

    public static StoredConversation ToStoredConversation(ConversationDTO conversation)
    {
        return new StoredConversation
        {
            ConversationId = conversation.ConversationId,
            ModelName = conversation.ModelName,
            ConversationName = conversation.ConversationName,
            CreatedAt = conversation.CreatedAt
        };
    }
}