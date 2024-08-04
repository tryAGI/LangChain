using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;

namespace LangChain.Databases.Qdrant
{
    public class QdrantVectorCollection(
        QdrantMemoryStore store,
        string name = VectorCollection.DefaultName,
        string? id = null) : SemanticKernelMemoryStoreCollection(store, name, id);
}
