using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve.Classes.DTO;

public class MessageDto
{
    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public Guid ConversationId { get; set; }

    public Guid MessageId { get; set; }

    public static MessageDto FromStoredMessage(StoredMessage message, string modelName)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));
        
        return new MessageDto
        {
            ConversationId = message.ConversationId,
            Author = message.Author == MessageAuthor.User
                ? "You"
                : modelName,
            Content = message.Content,
            MessageId = message.MessageId
        };
    }

    

    
}