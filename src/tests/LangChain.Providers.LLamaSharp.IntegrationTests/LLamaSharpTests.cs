using LangChain.Providers;
using LangChain.Providers.Downloader;
using LangChain.Providers.LLamaSharp;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestClass]
public class LLamaSharpTests
{
    string ModelPath=>HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf","main").Result;
    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void PrepromptTest()
    {
        var model = new LLamaSharpModelChat(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
        });

        var response=model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are simple assistant. If human say 'Bob' then you will respond with 'Jack'.".AsSystemMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
        })).Result;

        Assert.AreEqual("Jack",response.Messages.Last().Content );

    }

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void InstructionTest()
    {
        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature=0
        });

        var response = model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are a calculator. Print the result of this expression: 2 + 2.".AsSystemMessage(),
            "Result:".AsSystemMessage(),
        })).Result;

        Assert.AreEqual("4",response.Messages.Last().Content.Trim());

    }

    float VectorDistance(float[] a, float[] b)
    {
        float result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result += (a[i] - b[i]) * (a[i] - b[i]);
        }

        return result;

    }
    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void EmbeddingsTest()
    {
        var model = new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });

        string[] texts = new string[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        };

        var database = model.EmbedDocumentsAsync(texts).Result;


        var query = model.EmbedQueryAsync("How do you call your pet?").Result;

        var zipped = database.Zip(texts);

        var ordered= zipped.Select(x=>new {text=x.Second,dist=VectorDistance(x.First,query)});
        
        var closest = ordered.OrderBy(x => x.dist).First();

        Assert.AreEqual("My dog name is Bob", closest.text);
    }
}