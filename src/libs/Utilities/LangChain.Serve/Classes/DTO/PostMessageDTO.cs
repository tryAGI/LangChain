using LangChain.Utilities.Classes.Repository;

namespace LangChain.Serve.Classes.DTO;

public class PostMessageDTO
{
    public string Content { get; set; }

    public StoredMessage ToStoredMessage(Guid conversationId)
    {
        return new StoredMessage
        {
            ConversationId = conversationId,
            Author = MessageAuthor.User,
            Content = Content,
            MessageId = Guid.NewGuid()
        };
    }
}