using DotNet.Testcontainers.Builders;
using LangChain.Databases.Chroma;
using LangChain.Databases.InMemory;
using LangChain.Databases.OpenSearch;
using LangChain.Databases.Postgres;
using Testcontainers.PostgreSql;

namespace LangChain.Databases.IntegrationTests;

public partial class Tests
{
    private static async Task<TestEnvironment> StartEnvironmentForAsync(SupportedDatabase database, CancellationToken cancellationToken = default)
    {
        switch (database)
        {
            case SupportedDatabase.InMemory:
            {
                return new TestEnvironment
                {
                    VectorDatabase = new InMemoryVectorStore(),
                };
            }
            case SupportedDatabase.Chroma:
            {
                var port = Random.Shared.Next(49152, 65535);
                var collectionName = GenerateCollectionName();
                var container = new ContainerBuilder()
                    .WithImage("chromadb/chroma")
                    .WithPortBinding(hostPort: port, containerPort: 8000)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8000))
                    .Build();

                await container.StartAsync(cancellationToken);

                return new TestEnvironment
                {
                    VectorDatabase = new ChromaVectorStore(
                        new HttpClient(),
                        $"http://localhost:{port}",
                        collectionName),
                    Port = port,
                    CollectionName = collectionName,
                };
            }
            case SupportedDatabase.SqLite:
            {
                return new TestEnvironment
                {
                    VectorDatabase = new SQLiteVectorStore("vectors.db", GenerateCollectionName()),
                };
            }
            case SupportedDatabase.Postgres:
            {
                var container = new PostgreSqlBuilder()
                    .WithImage("pgvector/pgvector:pg16")
                    .WithPassword("password")
                    .WithDatabase("test")
                    .WithUsername("postgres")
                    .WithPortBinding(hostPort: 5433, containerPort: 5432)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
                    .Build();

                await container.StartAsync(cancellationToken);
                
                return new TestEnvironment
                {
                    VectorDatabase = new PostgresVectorDatabase(container.GetConnectionString(), vectorSize: 1536),
                    Container = container,
                };
            }
            case SupportedDatabase.OpenSearch:
            {
                const string password = "StronG#1235";
                
                var container = new ContainerBuilder()
                    .WithImage("opensearchproject/opensearch:latest")
                    .WithPortBinding(hostPort: 9600, containerPort: 9600) // multiple ports can be not supported
                    .WithPortBinding(hostPort: 9200, containerPort: 9200) // multiple ports can be not supported
                    .WithEnvironment("discovery.type", "single-node")
                    .WithEnvironment("plugins.security.disabled", "true")
                    .WithEnvironment("OPENSEARCH_INITIAL_ADMIN_PASSWORD", password)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9200))
                    .Build();
                
                await container.StartAsync(cancellationToken);

                return new TestEnvironment
                {
                    VectorDatabase = new OpenSearchVectorStore(new OpenSearchVectorStoreOptions
                    {
                        ConnectionUri = new Uri("http://localhost:9200"),
                        Username = "admin",
                        Password = password,
                        IndexName = GenerateCollectionName(),
                        Dimensions = 1024
                    }),
                };
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(database), database, null);
        }
    }
}