using LangChain.Databases.Connectors;
using Microsoft.SemanticKernel.Connectors.Milvus;

namespace LangChain.Databases.Milvus;

public class MilvusVectorDatabase(MilvusMemoryStore store) : SemanticKernelMemoryDatabase(store);