using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Sources;
using LangChain.Indexes;
using LangChain.Prompts;
using LangChain.Splitters.Text;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

public partial class LLamaSharpTests
{
    IEmbeddingModel CreateEmbeddings()
    {
        var embeddings = new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });
        return embeddings;
    }

    IChatModel CreateInstructionModel()
    {
        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });
        return model;

    }

    private IChatModel CreateChatModel()
    {
        var model = new LLamaSharpModelChat(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
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