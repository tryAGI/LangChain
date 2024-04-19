using LangChain.Databases.Chroma;
using LangChain.Databases.InMemory;

namespace LangChain.Databases.IntegrationTests;

public partial class Tests
{
    public static IVectorDatabase GetConfiguredVectorDatabase(SupportedDatabase database)
    {
        return database switch
        {
            SupportedDatabase.InMemory => (IVectorDatabase)new InMemoryVectorStore(),
            SupportedDatabase.Chroma => new ChromaVectorStore(new HttpClient(), "http://localhost:8000", GenerateCollectionName()),
            SupportedDatabase.SqLite => new SQLiteVectorStore("vectors.db", GenerateCollectionName()),
            _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
        };
    }
}