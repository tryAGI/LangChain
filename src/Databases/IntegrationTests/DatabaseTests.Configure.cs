using DotNet.Testcontainers.Builders;
using LangChain.Databases.Chroma;
using LangChain.Databases.InMemory;
using LangChain.Databases.OpenSearch;
using LangChain.Databases.Postgres;
using LangChain.Databases.Sqlite;
using Testcontainers.PostgreSql;

namespace LangChain.Databases.IntegrationTests;

public partial class DatabaseTests
{
    private static async Task<DatabaseTestEnvironment> StartEnvironmentForAsync(SupportedDatabase database, CancellationToken cancellationToken = default)
    {
        switch (database)
        {
            case SupportedDatabase.InMemory:
            {
                return new DatabaseTestEnvironment
                {
                    VectorDatabase = new InMemoryVectorDatabase(),
                };
            }
            case SupportedDatabase.Chroma:
            {
                var port = Random.Shared.Next(49152, 65535);
                var container = new ContainerBuilder()
                    .WithImage("chromadb/chroma")
                    .WithPortBinding(hostPort: port, containerPort: 8000)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8000))
                    .Build();

                await container.StartAsync(cancellationToken);

                return new DatabaseTestEnvironment
                {
                    VectorDatabase = new ChromaVectorDatabase(
                        new HttpClient(),
                        $"http://localhost:{port}"),
                    Container = container,
                    Port = port,
                };
            }
            case SupportedDatabase.SqLite:
            {
                return new DatabaseTestEnvironment
                {
                    VectorDatabase = new SqLiteVectorDatabase("vectors.db"),
                };
            }
            // In order to run tests please run postgres with installed pgvector locally
            // e.g. with docker <see href="https://github.com/pgvector/pgvector#additional-installation-methods"/>
            // docker run -p 5433:5432 -e POSTGRES_PASSWORD=password -e POSTGRES_DB=test ankane/pgvector
            case SupportedDatabase.Postgres:
            {
                var port = Random.Shared.Next(49152, 65535);
                var container = new PostgreSqlBuilder()
                    .WithImage("pgvector/pgvector:pg16")
                    .WithPassword("password")
                    .WithDatabase("test")
                    .WithUsername("postgres")
                    .WithPortBinding(hostPort: port, containerPort: 5432)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
                    .Build();
            
                await container.StartAsync(cancellationToken);
                
                return new DatabaseTestEnvironment
                {
                    VectorDatabase = new PostgresVectorDatabase(container.GetConnectionString()),
                    Container = container,
                };
            }
            case SupportedDatabase.OpenSearch:
            {
                const string password = "StronG#1235";
                
                var port1 = Random.Shared.Next(49152, 65535);
                var port2 = Random.Shared.Next(49152, 65535);
                var container = new ContainerBuilder()
                    .WithImage("opensearchproject/opensearch:latest")
                    .WithPortBinding(hostPort: port1, containerPort: 9600) // multiple ports can be not supported
                    .WithPortBinding(hostPort: port2, containerPort: 9200) // multiple ports can be not supported
                    .WithEnvironment("discovery.type", "single-node")
                    .WithEnvironment("plugins.security.disabled", "true")
                    .WithEnvironment("OPENSEARCH_INITIAL_ADMIN_PASSWORD", password)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9200))
                    .Build();
                
                await container.StartAsync(cancellationToken);
            
                return new DatabaseTestEnvironment
                {
                    VectorDatabase = new OpenSearchVectorDatabase(new OpenSearchVectorDatabaseOptions
                    {
                        ConnectionUri = new Uri($"http://localhost:{port2}"),
                        Username = "admin",
                        Password = password,
                    }),
                    Container = container,
                    Port = port2,
                };
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(database), database, null);
        }
    }
}