namespace LangChain.Utilities.Classes.Repository;

public class StoredMessage
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public MessageAuthor Author { get; set; }
    public string Content { get; set; } = string.Empty;
}