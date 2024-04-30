using DotNet.Testcontainers.Containers;
using LangChain.Providers;

namespace LangChain.Databases.IntegrationTests;

public sealed class DatabaseTestEnvironment : IAsyncDisposable
{
    public required IVectorDatabase VectorDatabase { get; set; }
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
        if (VectorDatabase is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}