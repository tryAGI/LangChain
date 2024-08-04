using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Pinecone;

namespace LangChain.Databases.Pinecone
{
    public class PineconeVectorDatabase(PineconeMemoryStore store) : SemanticKernelMemoryDatabase(store);
}
