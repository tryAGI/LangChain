using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve.Classes.DTO;

public class PostMessageDto
{
    public string Content { get; set; } = string.Empty;

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