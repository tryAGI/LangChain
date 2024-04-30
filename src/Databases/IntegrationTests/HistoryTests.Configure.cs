using DotNet.Testcontainers.Builders;
using LangChain.Databases.Mongo;
using LangChain.Databases.Mongo.Client;
using LangChain.Memory;
using Testcontainers.MongoDb;
using Testcontainers.Redis;

namespace LangChain.Databases.IntegrationTests;

public partial class HistoryTests
{
    private static async Task<HistoryTestEnvironment> StartEnvironmentForAsync(SupportedDatabase database, CancellationToken cancellationToken = default)
    {
        switch (database)
        {
            case SupportedDatabase.InMemory:
                {
                    return new HistoryTestEnvironment
                    {
                        History = new ChatMessageHistory(),
                    };
                }
            case SupportedDatabase.File:
                {
                    return new HistoryTestEnvironment
                    {
                        History = await FileChatMessageHistory.CreateAsync(Path.GetTempFileName(), cancellationToken),
                    };
                }
            case SupportedDatabase.Chroma:
                {
                    throw new NotImplementedException();
                }
            case SupportedDatabase.SqLite:
                {
                    throw new NotImplementedException();
                }
            case SupportedDatabase.Postgres:
                {
                    throw new NotImplementedException();
                }
            case SupportedDatabase.OpenSearch:
                {
                    throw new NotImplementedException();
                }
            case SupportedDatabase.Mongo:
                {
                    var container = new MongoDbBuilder()
                        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MongoDbBuilder.MongoDbPort))
                        .Build();

                    await container.StartAsync(cancellationToken);

                    return new HistoryTestEnvironment
                    {
                        History = new MongoChatMessageHistory(
                            "History",
                            new MongoDbClient(new MongoContext(new DatabaseConfiguration
                            {
                                ConnectionString = container.GetConnectionString(),
                                DatabaseName = "langchain",
                            }))
                        ),
                        Container = container,
                    };
                }
            case SupportedDatabase.Redis:
                {
                    var container = new RedisBuilder()
                        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(RedisBuilder.RedisPort))
                        .Build();

                    await container.StartAsync(cancellationToken);

                    return new HistoryTestEnvironment
                    {
                        History = new RedisChatMessageHistory(
                            sessionId: "History",
                            connectionString: container.GetConnectionString(),
                            ttl: TimeSpan.FromSeconds(30)),
                        Container = container,
                    };
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(database), database, null);
        }
    }
}