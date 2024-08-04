using LangChain.Databases.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.DuckDB;

namespace LangChain.Databases.DuckDb
{
    public class DuckDbVectorCollection(
        DuckDBMemoryStore store,
        string name = VectorCollection.DefaultName,
        string? id = null) : SemanticKernelMemoryStoreCollection(store, name, id);
}
