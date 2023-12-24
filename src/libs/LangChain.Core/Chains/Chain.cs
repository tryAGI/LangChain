using LangChain.Abstractions.Chains.Base;
using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains;
using LangChain.Indexes;
using LangChain.Memory;
using LangChain.Providers;

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
    public static PromptChain Template(string template,
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
    public static SetChain Set(object value, string outputKey = "value")
    {
        return new SetChain(value, outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueGetter"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static SetLambdaChain Set(Func<string> valueGetter, string outputKey = "value")
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
    public static LLMChain LLM(IChatModel llm,
        string inputKey = "prompt", string outputKey = "text")
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
        string inputKey = "documents", string outputKey = "combined")
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
    public static UpdateMemoryChain UpdateMemory(BaseChatMemory memory,
        string requestKey = "query", string responseKey = "text")
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
    public static TTSChain<T> TTS<T>(ITextToSpeechModel<T> model,
        T settings,
        string inputKey = "text", string outputKey = "audio")
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
    public static STTChain<T> STT<T>(ISpeechToTextModel<T> model,
        T settings,
        string inputKey = "audio", string outputKey = "text")
    {
        return new STTChain<T>(model, settings, inputKey, outputKey);
    }
}