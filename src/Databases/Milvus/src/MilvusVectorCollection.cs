using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Milvus;

namespace LangChain.Databases.Milvus;

public class MilvusVectorCollection(
        MilvusMemoryStore store,
        string name = VectorCollection.DefaultName,
        string? id = null) : SemanticKernelMemoryStoreCollection(store, name, id);