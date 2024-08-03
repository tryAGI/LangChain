using LangChain.Databases.Connectors;
using Microsoft.SemanticKernel.Connectors.Weaviate;

namespace LangChain.Databases.Weaviate
{
    public class WeaviateVectorDatabase(WeaviateMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
