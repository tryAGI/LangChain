using LangChain.Databases;
using LangChain.Docstore;
using LangChain.Providers.Downloader;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestClass]
public class SQLiteVectorStoreTest
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void SqliteTest()
    {
        
        var embeddings = LLamaSharpEmbeddings.FromPath(ModelPath);
        var documents = new string[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        }.ToDocuments();
        
        var dbExists = File.Exists("vectors.db");
        var store = new SQLiteVectorStore("vectors.db","vectors",embeddings);
        if (!dbExists)
        {
            store.AddDocumentsAsync(documents).Wait();
        }
        

        var results = store.AsRetreiver().GetRelevantDocumentsAsync("What is my dog name?").Result.ToList();

        Assert.AreEqual(results[0].PageContent, "My dog name is Bob");
    }
}