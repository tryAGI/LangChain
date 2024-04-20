using DotNet.Testcontainers.Containers;
using LangChain.Providers;

namespace LangChain.Databases.IntegrationTests;

public class TestEnvironment : IAsyncDisposable
{
    public required IVectorDatabase VectorDatabase { get; set; }
    public IContainer? Container { get; set; }
    public IEmbeddingModel EmbeddingModel { get; set; } = Tests.CreateEmbeddingModelMock().Object;
    
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