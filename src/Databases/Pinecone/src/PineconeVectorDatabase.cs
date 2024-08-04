using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Pinecone;

namespace LangChain.Databases.Pinecone
{
    public class MilvusVectorDatabase(PineconeMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
