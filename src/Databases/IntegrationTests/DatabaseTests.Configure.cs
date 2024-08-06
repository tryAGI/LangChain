using DotNet.Testcontainers.Builders;
using Elastic.Clients.Elasticsearch;
using LangChain.Databases.Chroma;
using LangChain.Databases.Elasticsearch;
using LangChain.Databases.InMemory;
using LangChain.Databases.Milvus;
using LangChain.Databases.Mongo;
using LangChain.Databases.OpenSearch;
using LangChain.Databases.Postgres;
using LangChain.Databases.Sqlite;
using Testcontainers.Elasticsearch;
using LangChain.Databases.DuckDb;
using Microsoft.SemanticKernel.Connectors.DuckDB;
using Microsoft.SemanticKernel.Connectors.Milvus;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using LangChain.Databases.Weaviate;
using Microsoft.SemanticKernel.Connectors.Weaviate;
using Testcontainers.Milvus;

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
                        VectorDatabase = new SqLiteVectorDatabase(dataSource: ":memory:"),
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

            case SupportedDatabase.Mongo:
                {
                    var port = Random.Shared.Next(49152, 65535);
                    var container = new MongoDbBuilder()
                        .WithImage("mongo")
                        .WithPortBinding(hostPort: port, containerPort: 27017)
                        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(27017))
                        .Build();

                    await container.StartAsync(cancellationToken);

                    return new DatabaseTestEnvironment
                    {
                        VectorDatabase = new MongoVectorDatabase(container.GetConnectionString()),
                        Container = container,
                    };
                }
            case SupportedDatabase.DuckDb:
                var store = await DuckDBMemoryStore.ConnectAsync(cancellationToken);
                return new DatabaseTestEnvironment
                {
                    VectorDatabase = new DuckDbVectorDatabase(store)
                };
            case SupportedDatabase.Elasticsearch:
                {
                    var container = new ElasticsearchBuilder().Build();

                    await container.StartAsync(cancellationToken);

                    var client = new ElasticsearchClient(new Uri($"http://localhost:{container.GetMappedPublicPort(9200)}"));
                    return new DatabaseTestEnvironment
                    {
                        VectorDatabase = new ElasticsearchVectorDatabase(client),
                        Container = container,
                    };
                }
            case SupportedDatabase.Milvus:
                {
                    var container = new MilvusBuilder().Build();
                    
                    await container.StartAsync(cancellationToken);
                    // var network = new NetworkBuilder()
                    //     .WithName("milvus-network")
                    //     .Build();
                    //
                    // var etcdContainer = new ContainerBuilder()
                    //     .WithImage("quay.io/coreos/etcd:v3.5.5")
                    //     .WithName("milvus-etcd")
                    //     .WithEnvironment("ETCD_AUTO_COMPACTION_MODE", "revision")
                    //     .WithEnvironment("ETCD_AUTO_COMPACTION_RETENTION", "1000")
                    //     .WithEnvironment("ETCD_QUOTA_BACKEND_BYTES", "4294967296")
                    //     .WithEnvironment("ETCD_SNAPSHOT_COUNT", "50000")
                    //     .WithPortBinding(2379, 2379)
                    //     .WithCommand("etcd",
                    //                  "-advertise-client-urls=http://0.0.0.0:2379",
                    //                  "-listen-client-urls=http://0.0.0.0:2379",
                    //                  "--data-dir", "/etcd")
                    //     .Build();
                    //
                    // var minioContainer = new ContainerBuilder()
                    //     .WithImage("minio/minio:RELEASE.2023-03-20T20-16-18Z")
                    //     .WithName("milvus-minio")
                    //     .WithPortBinding(9000, 9000)
                    //     .WithPortBinding(9001, 9001)
                    //     .WithEnvironment("MINIO_ACCESS_KEY", "minioadmin")
                    //     .WithEnvironment("MINIO_SECRET_KEY", "minioadmin")
                    //     .WithCommand("minio", "server", "/minio_data", "--console-address", ":9001")
                    //     .Build();
                    //
                    // var milvusContainer = new ContainerBuilder()
                    //     .WithImage("milvusdb/milvus:v2.3.0")
                    //     .WithName("milvus-standalone")
                    //     .WithPortBinding(19530, 19530)
                    //     .WithPortBinding(9091, 9091)
                    //     .WithEnvironment("ETCD_ENDPOINTS", "milvus-etcd:2379")
                    //     .WithEnvironment("MINIO_ADDRESS", "milvus-minio:9000")
                    //     .WithCommand("milvus", "run", "standalone")
                    //     .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(2379))
                    //     .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9000))
                    //     .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9001))
                    //     .DependsOn(etcdContainer)
                    //     .DependsOn(minioContainer)
                    //     .Build();
                    //
                    // await etcdContainer.StartAsync(cancellationToken);
                    // await minioContainer.StartAsync(cancellationToken);
                    // await milvusContainer.StartAsync(cancellationToken);

                    return new DatabaseTestEnvironment
                    {
                        VectorDatabase = new MilvusVectorDatabase(new MilvusMemoryStore("localhost", port: container.GetMappedPublicPort(19530))),
                        Container = container,
                    };
                }
            case SupportedDatabase.Weaviate:
                {
                    var port1 = Random.Shared.Next(49152, 65535);
                    var port2 = Random.Shared.Next(49152, 65535);
                    var container = new ContainerBuilder()
                        .WithImage("cr.weaviate.io/semitechnologies/weaviate:1.25.10")
                        .WithPortBinding(hostPort: port1, containerPort: 8080)
                        .WithPortBinding(hostPort: port2, containerPort: 50051)
                        .WithEnvironment("AUTHENTICATION_ANONYMOUS_ACCESS_ENABLED", "true")
                        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
                        .Build();
                    
                    await container.StartAsync(cancellationToken);
                    
                    await Task.Delay(5000, cancellationToken);
                    
                    return new DatabaseTestEnvironment
                    {
                        VectorDatabase = new WeaviateVectorDatabase(new WeaviateMemoryStore($"http://localhost:{port1}")),
                        Container = container
                    };
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(database), database, null);
        }
    }
}