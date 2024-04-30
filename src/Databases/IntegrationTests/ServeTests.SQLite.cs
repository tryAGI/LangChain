using LangChain.Databases.Sqlite;
using LangChain.Serve.Abstractions;
using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Databases.IntegrationTests;

[TestFixture]
public partial class ServeTests
{
    [Test]
    public async Task Test1()
    {
        IConversationRepository repository = new SqLiteConversationRepository("Data Source=:memory:;");

        // setup
        var conversation = await repository.CreateConversation("test");
        await repository.AddMessage(new StoredMessage
        {
            Content = "message1",
            Author = MessageAuthor.User,
            ConversationId = conversation.ConversationId,
            MessageId = Guid.NewGuid(),
        });
        await repository.AddMessage(new StoredMessage
        {
            Content = "message2",
            Author = MessageAuthor.Ai,
            ConversationId = conversation.ConversationId,
            MessageId = Guid.NewGuid(),
        });

        // retrieve
        var conversations = await repository.ListConversations();
        var messages = await repository.ListMessages(conversations.First().ConversationId);

        // assert
        conversations.Count.Should().Be(1);
        messages.Count.Should().Be(2);
        messages.First().Content.Should().Be("message1");
        messages.Last().Content.Should().Be("message2");
        messages.First().Author.Should().Be(MessageAuthor.User);
        messages.Last().Author.Should().Be(MessageAuthor.Ai);
    }
}