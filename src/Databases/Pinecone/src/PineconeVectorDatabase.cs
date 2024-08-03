using LangChain.Databases.Connectors;
using Microsoft.SemanticKernel.Connectors.Pinecone;

namespace LangChain.Databases.Pinecone
{
    public class MilvusVectorDatabase(PineconeMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
