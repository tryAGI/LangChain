using DotNet.Testcontainers.Builders;
using LangChain.Databases.Mongo;
using LangChain.Databases.Mongo.Client;
using LangChain.Providers;
using Testcontainers.MongoDb;

namespace LangChain.Databases.IntegrationTests;

public partial class Tests
{
    [Test]
    public async Task GetMessages_EmptyHistory_Ok()
    {
        await using var container = new MongoDbBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MongoDbBuilder.MongoDbPort))
            .Build();

        await container.StartAsync();

        var mongoClient = new MongoDbClient(new MongoContext(new DatabaseConfiguration
        {
            ConnectionString = container.GetConnectionString(),
            DatabaseName = "langchain",
        }));
        
        const string sessionId = "GetMessages_EmptyHistory_Ok";
        var history = new MongoChatMessageHistory(
            sessionId,
            mongoClient
        );

        history.Messages.Should().BeEmpty();
    }
    
    [Test]
    public async Task AddMessage_Ok()
    {
        await using var container = new MongoDbBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MongoDbBuilder.MongoDbPort))
            .Build();

        await container.StartAsync();

        var mongoClient = new MongoDbClient(new MongoContext(new DatabaseConfiguration
        {
            ConnectionString = container.GetConnectionString(),
            DatabaseName = "langchain",
        }));
        
        const string sessionId = "MongoChatMessageHistoryTests_AddMessage_Ok";
        var history = new MongoChatMessageHistory(
            sessionId,
            mongoClient);
    
        var humanMessage = Message.Human("Hi, AI");
        await history.AddMessage(humanMessage);
        var aiMessage = Message.Ai("Hi, human");
        await history.AddMessage(aiMessage);
    
        var actual = history.Messages;
    
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(2);
        actual[0].Role.Should().Be(humanMessage.Role);
        actual[0].Content.Should().Be(humanMessage.Content);
        actual[1].Role.Should().Be(aiMessage.Role);
        actual[1].Content.Should().Be(aiMessage.Content);       
    }
    
    [Test]
    public async Task Clear_Ok()
    {
        await using var container = new MongoDbBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MongoDbBuilder.MongoDbPort))
            .Build();

        await container.StartAsync();

        var mongoClient = new MongoDbClient(new MongoContext(new DatabaseConfiguration
        {
            ConnectionString = container.GetConnectionString(),
            DatabaseName = "langchain",
        }));

        const string sessionId = "clear_ok";
        var history = new MongoChatMessageHistory(
            sessionId,
            mongoClient);
    
        await history.Clear();
    
        history.Messages.Should().BeEmpty();
    }
}