using LangChain.Memory;

var inMemoryHistory = new ChatMessageHistory();

await inMemoryHistory.AddUserMessage("hi!");

await inMemoryHistory.AddAiMessage("whats up?");

foreach (var message in inMemoryHistory.Messages)
{
    Console.WriteLine(message.GetType().Name + ":" + message.Content);
}

