using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains;
using LangChain.Chains.StackableChains.Agents;
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
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static PromptChain Template(
        string template,
        string outputKey = "prompt")
    {
        return new PromptChain(template, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static SetChain Set(
        object value,
        string outputKey = "value")
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
        string outputKey = "value")
    {
        return new SetLambdaChain(valueGetter, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="llm"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static LLMChain LLM(
        IChatModel llm,
        string inputKey = "prompt",
        string outputKey = "text")
    {
        return new LLMChain(llm, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static RetrieveDocumentsChain RetrieveDocuments(
        VectorStoreIndexWrapper index,
        string inputKey = "query",
        string outputKey = "documents")
    {
        return new RetrieveDocumentsChain(index, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static StuffDocumentsChain StuffDocuments(
        string inputKey = "documents",
        string outputKey = "combined")
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
        string requestKey = "query",
        string responseKey = "text")
    {
        return new UpdateMemoryChain(memory, requestKey, responseKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="settings"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static TTSChain<T> TTS<T>(
        ITextToSpeechModel<T> model,
        T settings,
        string inputKey = "text",
        string outputKey = "audio")
    {
        return new TTSChain<T>(model, settings, inputKey, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="settings"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static STTChain<T> STT<T>(
        ISpeechToTextModel<T> model,
        T settings,
        string inputKey = "audio",
        string outputKey = "text")
    {
        return new STTChain<T>(model, settings, inputKey, outputKey);
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
        string inputKey = "input",
        string outputKey = "final_answer")
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
        string outputKey = "answer")
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
        string inputKey = "input",
        string outputKey = "output")
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
        IGenerateImageModel model,
        string inputKey = "prompt",
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
}