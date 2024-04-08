using LangChain.Databases;
using LangChain.Sources;
using LangChain.Providers.HuggingFace.Downloader;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public class SQLiteVectorStoreTest
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [Test]
    [Explicit]
    public async Task SqliteTest()
    {
        var embeddings = LLamaSharpEmbeddings.FromPath(ModelPath);
        
        var dbExists = File.Exists("vectors.db");
        var vectorDatabase = new SQLiteVectorStore("vectors.db","vectors");
        if (!dbExists)
        {
            await vectorDatabase.AddDocumentsAsync(embeddings, new string[]
            {
                "I spent entire day watching TV",
                "My dog name is Bob",
                "This icecream is delicious",
                "It is cold in space"
            }.ToDocuments());
        }
        

        var results = await vectorDatabase.AsRetriever(embeddings).GetRelevantDocumentsAsync("What is my dog name?");

        results.ElementAt(0).PageContent.Should().Be("My dog name is Bob");
    }
}