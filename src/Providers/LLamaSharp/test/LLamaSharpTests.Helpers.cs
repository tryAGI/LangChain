using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Prompts;
using LangChain.Providers.HuggingFace.Downloader;
using LangChain.Splitters.Text;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

public partial class LLamaSharpTests
{
    async Task<IEmbeddingModel> CreateEmbeddingsAsync()
    {
        var modelPath = await HuggingFaceModelDownloader.GetModelAsync(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");
        var embeddings = new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = modelPath,
            Temperature = 0
        });
        return embeddings;
    }

    async Task<IChatModel> CreateInstructionModelAsync()
    {
        var modelPath = await HuggingFaceModelDownloader.GetModelAsync(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");
        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = modelPath,
            Temperature = 0
        });
        return model;

    }

    private async Task<IChatModel> CreateChatModelAsync()
    {
        var modelPath = await HuggingFaceModelDownloader.GetModelAsync(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");
        var model = new LLamaSharpModelChat(new LLamaSharpConfiguration
        {
            PathToModelFile = modelPath,
            Temperature = 0
        });
        return model;
    }

    private static async Task<IVectorCollection> CreateVectorStoreIndex(IEmbeddingModel embeddings, string[] texts)
    {
        var vectorDatabase = new InMemoryVectorCollection();
        await vectorDatabase.AddSplitDocumentsAsync(
            embeddings,
            texts.ToDocuments(),
            new CharacterTextSplitter());

        return vectorDatabase;
    }

    PromptTemplate CreatePromptTemplate()
    {
        string prompt = "Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.\n\n{context}\n\nQuestion: {question}\nHelpful Answer:";
        var template = new PromptTemplate(new PromptTemplateInput(prompt, new List<string>() { "context", "question" }));
        return template;
    }

}