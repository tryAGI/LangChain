﻿using LangChain.Databases;
using LangChain.Sources;
using LangChain.Providers.HuggingFace.Downloader;
using LangChain.VectorStores;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public class SQLiteVectorStoreTest
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [Test]
    [Explicit]
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
        

        var results = store.AsRetriever().GetRelevantDocumentsAsync("What is my dog name?").Result.ToList();

        results[0].PageContent.Should().Be("My dog name is Bob");
    }
}