using DotNet.Testcontainers.Containers;
using LangChain.Memory;
using LangChain.Providers;

namespace LangChain.Databases.IntegrationTests;

public sealed class HistoryTestEnvironment : IAsyncDisposable
{
    public required BaseChatMessageHistory History { get; set; }
    public int Port { get; set; }
    public string CollectionName { get; set; } = "test" + Guid.NewGuid().ToString("N");
    public int Dimensions { get; set; } = 1536;
    public IContainer? Container { get; set; }
    public IEmbeddingModel EmbeddingModel { get; set; } = DatabaseTests.CreateEmbeddingModelMock().Object;

    public async ValueTask DisposeAsync()
    {
        if (Container != null)
        {
            await Container.DisposeAsync();
        }
    }
}