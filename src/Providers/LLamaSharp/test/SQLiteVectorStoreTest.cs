using LangChain.Databases;
using LangChain.Databases.Sqlite;
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
        var vectorDatabase = new SqLiteVectorDatabase("vectors.db");
        var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, dimensions: 1536);
        if (!dbExists)
        {
            await vectorCollection.AddDocumentsAsync(embeddings, new string[]
            {
                "I spent entire day watching TV",
                "My dog name is Bob",
                "This icecream is delicious",
                "It is cold in space"
            }.ToDocuments());
        }


        var results = await vectorCollection.AsRetriever(embeddings).GetRelevantDocumentsAsync("What is my dog name?");

        results.ElementAt(0).PageContent.Should().Be("My dog name is Bob");
    }
}