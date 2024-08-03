using LangChain.Databases.Connectors;
using Microsoft.SemanticKernel.Connectors.Qdrant;

namespace LangChain.Databases.Qdrant
{
    public class QdrantVectorDatabase(QdrantMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
