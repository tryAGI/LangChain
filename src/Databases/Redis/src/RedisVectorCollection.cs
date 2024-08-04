using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;

namespace LangChain.Databases.Redis
{
    public class RedisVectorCollection(
    RedisMemoryStore store,
    string name = VectorCollection.DefaultName,
    string? id = null) : SemanticKernelMemoryStoreCollection(store, name, id);
}
