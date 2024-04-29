using DotNet.Testcontainers.Builders;
using LangChain.Memory;
using LangChain.Providers;
using Testcontainers.Redis;

namespace LangChain.Databases.IntegrationTests;

[TestFixture]
public class RedisTests
{
    [Test]
    public async Task GetMessages_EmptyHistory_Ok()
    {
        await using var container = new RedisBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(RedisBuilder.RedisPort))
            .Build();

        await container.StartAsync();

        BaseChatMessageHistory history = new RedisChatMessageHistory(
            sessionId: "GetMessages_EmptyHistory_Ok",
            connectionString: container.GetConnectionString(),
            ttl: TimeSpan.FromSeconds(30));

        var existing = history.Messages;

        existing.Should().BeEmpty();
    }

    [Test]
    public async Task AddMessage_Ok()
    {
        await using var container = new RedisBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(RedisBuilder.RedisPort))
            .Build();

        await container.StartAsync();

        BaseChatMessageHistory history = new RedisChatMessageHistory(
            sessionId: "RedisChatMessageHistoryTests_AddMessage_Ok",
            connectionString: container.GetConnectionString(),
            ttl: TimeSpan.FromSeconds(30));

        var humanMessage = Message.Human("Hi, AI");
        await history.AddMessage(humanMessage);
        var aiMessage = Message.Ai("Hi, human");
        await history.AddMessage(aiMessage);

        var actual = history.Messages;

        actual.Should().HaveCount(2);

        actual[0].Role.Should().Be(humanMessage.Role);
        actual[0].Content.Should().BeEquivalentTo(humanMessage.Content);

        actual[1].Role.Should().Be(aiMessage.Role);
        actual[1].Content.Should().BeEquivalentTo(aiMessage.Content);
    }

    [Test]
    public async Task Ttl_Ok()
    {
        await using var container = new RedisBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(RedisBuilder.RedisPort))
            .Build();

        await container.StartAsync();

        BaseChatMessageHistory history = new RedisChatMessageHistory(
            sessionId: "Ttl_Ok",
            connectionString: container.GetConnectionString(),
            ttl: TimeSpan.FromSeconds(2));

        var humanMessage = Message.Human("Hi, AI");
        await history.AddMessage(humanMessage);

        await Task.Delay(2_500);

        var existing = history.Messages;

        existing.Should().BeEmpty();
    }

    [Test]
    public async Task Clear_Ok()
    {
        await using var container = new RedisBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(RedisBuilder.RedisPort))
            .Build();

        await container.StartAsync();

        BaseChatMessageHistory history = new RedisChatMessageHistory(
            sessionId: "Clear_Ok",
            connectionString: container.GetConnectionString(),
            ttl: TimeSpan.FromSeconds(30));

        await history.Clear();

        var existing = history.Messages;

        existing.Should().BeEmpty();
    }
}