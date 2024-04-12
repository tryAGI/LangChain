namespace LangChain.Serve.Abstractions.Repository;

public class StoredConversation
{
    public Guid ConversationId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string? ConversationName { get; set; }
    public DateTime CreatedAt { get; set; }
}