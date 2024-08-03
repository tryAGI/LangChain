using LangChain.Databases.Connectors;
using Microsoft.SemanticKernel.Connectors.Redis;

namespace LangChain.Databases.Redis
{
    public class RedisVectorDatabase(RedisMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
