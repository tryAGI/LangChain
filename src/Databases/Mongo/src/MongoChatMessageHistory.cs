using LangChain.Databases.Mongo.Client;
using LangChain.Memory;
using LangChain.Databases.Mongo.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using LangChain.Providers;

namespace LangChain.Databases.Mongo;

public class MongoChatMessageHistory(
    string sessionId,
    IMongoDbClient mongoRepository)
    : BaseChatMessageHistory, IMongoChatMessageHistory
{
    protected IMongoDbClient MongoRepository { get; } = mongoRepository;

    public override async Task Clear()
    {
        await MongoRepository
            .BatchDeactivate<LangChainAiSessionHistory>(i => i.SessionId == sessionId).ConfigureAwait(false);
    }

    public override async Task AddMessage(Message message)
    {
        await MongoRepository.InsertAsync(new LangChainAiSessionHistory
        {
            SessionId = sessionId,
            Message = JsonSerializer.Serialize(message, SourceGenerationContext.Default.Message),
        }).ConfigureAwait(false);
    }

    public override IReadOnlyList<Message> Messages
    {
        get
        {
            return MongoRepository
                .GetSync<LangChainAiSessionHistory, string>(s =>
                        s.SessionId == sessionId &&
                        s.IsActive,
                        m => m.Message)
                .Select(static x => JsonSerializer.Deserialize(x.ToString(), SourceGenerationContext.Default.Message))
                .ToList();
        }
    }
}

[JsonSerializable(typeof(Message))]
public sealed partial class SourceGenerationContext : JsonSerializerContext;