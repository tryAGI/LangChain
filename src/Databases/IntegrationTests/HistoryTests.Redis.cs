using LangChain.Providers;

namespace LangChain.Databases.IntegrationTests;

[TestFixture]
public partial class HistoryTests
{
    [Test]
    public async Task Redis_Ttl_Ok()
    {
        await using var environment = await StartEnvironmentForAsync(SupportedDatabase.Redis);
        var history = (RedisChatMessageHistory)environment.History;
        history.Ttl = TimeSpan.FromSeconds(2);

        var humanMessage = Message.Human("Hi, AI");
        await history.AddMessage(humanMessage);

        await Task.Delay(2_500);

        var existing = history.Messages;

        existing.Should().BeEmpty();
    }
}