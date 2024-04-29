using LangChain.Providers;

namespace LangChain.Databases.IntegrationTests;

/// <summary>
/// In order to run tests please run redis locally, e.g. with docker
/// docker run -p 6379:6379 redis
/// </summary>
[TestFixture]
public class RedisTests
{
    private readonly string _connectionString = "127.0.0.1:6379";

    [Test]
    [Explicit]
    public void GetMessages_EmptyHistory_Ok()
    {
        var sessionId = "GetMessages_EmptyHistory_Ok";
        var history = new RedisChatMessageHistory(
            sessionId,
            _connectionString,
            ttl: TimeSpan.FromSeconds(30));

        var existing = history.Messages;

        existing.Should().BeEmpty();
    }

    [Test]
    [Explicit]
    public async Task AddMessage_Ok()
    {
        var sessionId = "RedisChatMessageHistoryTests_AddMessage_Ok";
        var history = new RedisChatMessageHistory(
            sessionId,
            _connectionString,
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
    [Explicit]
    public async Task Ttl_Ok()
    {
        var sessionId = "Ttl_Ok";
        var history = new RedisChatMessageHistory(
            sessionId,
            _connectionString,
            ttl: TimeSpan.FromSeconds(2));

        var humanMessage = Message.Human("Hi, AI");
        await history.AddMessage(humanMessage);

        await Task.Delay(2_500);

        var existing = history.Messages;

        existing.Should().BeEmpty();
    }

    [Test]
    [Explicit]
    public async Task Clear_Ok()
    {
        var sessionId = "Ttl_Ok";
        var history = new RedisChatMessageHistory(
            sessionId,
            _connectionString,
            ttl: TimeSpan.FromSeconds(30));

        await history.Clear();

        var existing = history.Messages;

        existing.Should().BeEmpty();
    }
}