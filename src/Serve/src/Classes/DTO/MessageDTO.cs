using LangChain.Utilities.Classes.Repository;

namespace LangChain.Serve.Classes.DTO;

public class MessageDTO
{
    public string Content { get; set; }

    public string Author { get; set; }

    public Guid ConversationId { get; set; }

    public Guid MessageId { get; set; }

    public static MessageDTO FromStoredMessage(StoredMessage message, string modelName)
    {
        return new MessageDTO
        {
            ConversationId = message.ConversationId,
            Author = message.Author==MessageAuthor.User?"You":modelName,
            Content = message.Content,
            MessageId = message.MessageId
        };
    }

    

    
}