using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Pinecone;

namespace LangChain.Databases.Pinecone
{
    public class PineconeVectorCollection(
        PineconeMemoryStore store,
        string name = VectorCollection.DefaultName,
        string? id = null) : SemanticKernelMemoryStoreCollection(store, name, id);
}
