namespace LangChain.Utilities.Classes.Repository;

public class StoredConversation
{
    public Guid ConversationId { get; set; }
    public string ModelName { get; set; }
    public string? ConversationName { get; set; }
    public DateTime CreatedAt { get; set; }
}