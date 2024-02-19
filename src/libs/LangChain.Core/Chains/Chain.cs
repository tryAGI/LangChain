using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains;
using LangChain.Chains.StackableChains.Agents;
using LangChain.Chains.StackableChains.Agents.Crew;
using LangChain.Chains.StackableChains.Files;
using LangChain.Chains.StackableChains.ImageGeneration;
using LangChain.Chains.StackableChains.ReAct;
using LangChain.Indexes;
using LangChain.Memory;
using LangChain.Providers;

// ReSharper disable InconsistentNaming

namespace LangChain.Chains;

/// <summary>
/// 
/// </summary>
public static class Chain
{
    /// <summary>
    /// Replaces context and question in the prompt with their values.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static PromptChain Template(
        string template,
        string outputKey = "text")
    {
        return new PromptChain(template, outputKey);
    }

    /// <summary>
    /// Sets the value to the context.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static SetChain Set(
        object value,
        string outputKey = "text")
    {
        return new SetChain(value, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueGetter"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static SetLambdaChain Set(
        Func<string> valueGetter,
        string outputKey = "text")
    {
        return new SetLambdaChain(valueGetter, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static DoChain Do(
        Action<Dictionary<string, object>> func)
    {
        return new DoChain(func);
    }

    /// <summary>
    /// Sends the result to the language model.
    /// </summary>
    /// <param name="llm"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static LLMChain LLM(
        IChatModel llm,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new LLMChain(llm, inputKey, outputKey);
    }

    /// <inheritdoc cref="LLM"/>
    public static LLMChain LargeLanguageModel(
        IChatModel llm,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new LLMChain(llm, inputKey, outputKey);
    }

    /// <inheritdoc cref="RetrieveSimilarDocuments"/>
    public static RetrieveDocumentsChain RetrieveDocuments(
        VectorStoreIndexWrapper index,
        int amount = 4,
        string inputKey = "text",
        string outputKey = "docs")
    {
        return new RetrieveDocumentsChain(index, inputKey, outputKey, amount);
    }


    /// <summary>
    /// Takes most similar documents.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="amount"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static RetrieveDocumentsChain RetrieveSimilarDocuments(
        VectorStoreIndexWrapper index,
        int amount = 4,
        string inputKey = "text",
        string outputKey = "docs")
    {
        return new RetrieveDocumentsChain(index, inputKey, outputKey, amount);
    }
    
    /// <inheritdoc cref="CombineDocuments"/>
    public static StuffDocumentsChain StuffDocuments(
        string inputKey = "docs",
        string outputKey = "c")
    {
        return new StuffDocumentsChain(inputKey, outputKey);
    }

    /// <summary>
    /// Combines documents together and put them into context
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static StuffDocumentsChain CombineDocuments(
        string inputKey = "docs",
        string outputKey = "c")
    {
        return new StuffDocumentsChain(inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="memory"></param>
    /// <param name="requestKey"></param>
    /// <param name="responseKey"></param>
    /// <returns></returns>
    public static UpdateMemoryChain UpdateMemory(
        BaseChatMemory memory,
        string requestKey = "text",
        string responseKey = "text")
    {
        return new UpdateMemoryChain(memory, requestKey, responseKey);
    }

    public static LoadMemoryChain LoadMemory(
        BaseChatMemory memory,
        string outputKey = "text")
    {
        return new LoadMemoryChain(memory, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="settings"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static TTSChain TTS(
        ITextToSpeechModel model,
        TextToSpeechSettings? settings = null,
        string inputKey = "text",
        string outputKey = "audio")
    {
        return new TTSChain(model, settings, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="settings"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static STTChain STT(
        ISpeechToTextModel model,
        SpeechToTextSettings? settings = null,
        string inputKey = "audio",
        string outputKey = "text")
    {
        return new STTChain(model, settings, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="reActPrompt"></param>
    /// <param name="maxActions"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ReActAgentExecutorChain ReActAgentExecutor(
        IChatModel model,
        string? reActPrompt = null,
        int maxActions = 5,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new ReActAgentExecutorChain(model, reActPrompt, maxActions, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ReActParserChain ReActParser(
        string inputKey = "text",
        string outputKey = "text")
    {
        return new ReActParserChain(inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="agents"></param>
    /// <param name="stopPhrase"></param>
    /// <param name="messagesLimit"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static GroupChat GroupChat(
        IList<AgentExecutorChain> agents,
        string? stopPhrase = null,
        int messagesLimit = 10,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new GroupChat(agents, stopPhrase, messagesLimit, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ImageGenerationChain GenerateImage(
        IImageGenerationModel model,
        string inputKey = "text",
        string outputKey = "image")
    {
        return new ImageGenerationChain(model, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="inputKey"></param>
    /// <returns></returns>
    public static SaveIntoFileChain SaveIntoFile(
        string path,
        string inputKey = "data")
    {
        return new SaveIntoFileChain(path, inputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="allAgents"></param>
    /// <param name="manager"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static CrewChain Crew(
        IEnumerable<CrewAgent> allAgents, CrewAgent manager,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new CrewChain(allAgents, manager, inputKey, outputKey);
    }

    public static ExtractCodeChain ExtractCode(
        string inputKey = "text",
        string outputKey = "code")
    {
        return new ExtractCodeChain(inputKey, outputKey);
    }
}
