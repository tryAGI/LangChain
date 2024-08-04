using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Weaviate;

namespace LangChain.Databases.Weaviate
{
    public class WeaviateVectorCollection(
    WeaviateMemoryStore store,
    string name = VectorCollection.DefaultName,
    string? id = null) : SemanticKernelMemoryStoreCollection(store, name, id);
}
