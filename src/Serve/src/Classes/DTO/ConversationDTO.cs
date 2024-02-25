using LangChain.Utilities.Classes.Repository;

namespace LangChain.Serve.Classes.DTO;

public class ConversationDTO
{
    public Guid ConversationId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string? ConversationName { get; set; }
    public DateTime CreatedAt { get; set; }

    public static ConversationDTO FromStoredConversation(StoredConversation conversation)
    {
        conversation = conversation ?? throw new ArgumentNullException(nameof(conversation));
        
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
        conversation = conversation ?? throw new ArgumentNullException(nameof(conversation));

        return new StoredConversation
        {
            ConversationId = conversation.ConversationId,
            ModelName = conversation.ModelName,
            ConversationName = conversation.ConversationName,
            CreatedAt = conversation.CreatedAt
        };
    }
}