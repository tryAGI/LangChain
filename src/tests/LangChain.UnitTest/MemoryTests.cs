using LangChain.Memory;
using LangChain.Providers;

namespace LangChain.UnitTest;

[TestFixture]
public class MemoryTests
{
    [Test]
    public void TestInMemoryHistory_WhenAddingMessages_ShouldStoreInMememory()
    {
        ChatMessageHistory inMemoryHistory = CreateInMemoryHistoryExample();

        inMemoryHistory.Messages.Should().HaveCount(2);

        inMemoryHistory.Messages.FirstOrDefault(x => x.Content == "hi!").Role.Should().Be(MessageRole.Human);
        inMemoryHistory.Messages.FirstOrDefault(x => x.Content == "whats up?").Role.Should().Be(MessageRole.Ai);
    }

    [Test]
    public void TestInMemoryHistory_WhenCleanMethodIsCalled_ShouldCleanHistory()
    {
        ChatMessageHistory inMemoryHistory = CreateInMemoryHistoryExample();

        inMemoryHistory.Clear();

        inMemoryHistory.Messages.Should().HaveCount(0);
    }

    private static ChatMessageHistory CreateInMemoryHistoryExample()
    {
        var inMemoryHistory = new ChatMessageHistory();

        inMemoryHistory.AddUserMessage("hi!");

        inMemoryHistory.AddAiMessage("whats up?");
        return inMemoryHistory;
    }
}

