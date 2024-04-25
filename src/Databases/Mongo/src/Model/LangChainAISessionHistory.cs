using LangChain.Databases.Mongo.Client;
using MongoDB.Bson.Serialization.Attributes;

namespace LangChain.Databases.Mongo.Model;

[BsonCollection("langchain_ai_session_history")]
[BsonIgnoreExtraElements]
public class LangChainAiSessionHistory : BaseEntity
{
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}