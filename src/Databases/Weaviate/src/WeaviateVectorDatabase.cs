using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Weaviate;

namespace LangChain.Databases.Weaviate
{
    public class WeaviateVectorDatabase(WeaviateMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
