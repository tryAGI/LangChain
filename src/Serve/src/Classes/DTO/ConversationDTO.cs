using LangChain.Serve.Classes.Repository;

namespace LangChain.Serve.Classes.DTO;

public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string? ConversationName { get; set; }
    public DateTime CreatedAt { get; set; }

    public static ConversationDto FromStoredConversation(StoredConversation conversation)
    {
        conversation = conversation ?? throw new ArgumentNullException(nameof(conversation));
        
        return new ConversationDto
        {
            ConversationId = conversation.ConversationId,
            ModelName = conversation.ModelName,
            ConversationName = conversation.ConversationName,
            CreatedAt = conversation.CreatedAt
        };
    }

    public static StoredConversation ToStoredConversation(ConversationDto conversation)
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