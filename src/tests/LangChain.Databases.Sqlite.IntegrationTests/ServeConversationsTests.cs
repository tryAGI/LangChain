using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Databases.Sqlite.IntegrationTests;

[TestFixture]
public class ServeConversationsTests
{
    [Test]
    public async Task Test1()
    {
        var sQLiteConversationRepository = new SqLiteConversationRepository("Data Source=:memory:;");
            
        // setup
        var conversation = await sQLiteConversationRepository.CreateConversation("test");
        await sQLiteConversationRepository.AddMessage(new StoredMessage()
        {
            Content = "message1",
            Author = MessageAuthor.User,
            ConversationId = conversation.ConversationId,
            MessageId = Guid.NewGuid(),
        });
        await sQLiteConversationRepository.AddMessage(new StoredMessage()
        {
            Content = "message2",
            Author = MessageAuthor.Ai,
            ConversationId = conversation.ConversationId,
            MessageId = Guid.NewGuid(),
        });

        // retreive
        var conversations = await sQLiteConversationRepository.ListConversations();
        var messages = await sQLiteConversationRepository.ListMessages(conversations.First().ConversationId);

        // assert
        Assert.That(conversations.Count(), Is.EqualTo(1));
        Assert.That(messages.Count(), Is.EqualTo(2));
        Assert.That(messages.First().Content, Is.EqualTo("message1"));
        Assert.That(messages.Last().Content, Is.EqualTo("message2"));
        Assert.That(messages.First().Author, Is.EqualTo(MessageAuthor.User));
        Assert.That(messages.Last().Author, Is.EqualTo(MessageAuthor.Ai));


        Assert.Pass();
    }
}