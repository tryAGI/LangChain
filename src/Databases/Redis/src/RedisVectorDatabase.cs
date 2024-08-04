using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;

namespace LangChain.Databases.Redis
{
    public class RedisVectorDatabase(RedisMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
