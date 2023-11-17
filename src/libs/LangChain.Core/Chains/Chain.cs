using LangChain.Abstractions.Chains.Base;
using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains;
using LangChain.Indexes;
using LangChain.Memory;
using LangChain.Providers;

namespace LangChain.Chains;

public static class Chain
{
    public static PromptChain Template(string template,
        string outputKey = "prompt")
    {
        return new PromptChain(template, outputKey);
    }

    public static SetChain Set(string value, string outputKey = "value")
    {
        return new SetChain(value, outputKey);
    }

    public static SetLambdaChain Set(Func<string> valueGetter, string outputKey = "value")
    {
        return new SetLambdaChain(valueGetter, outputKey);
    }

    public static LLMChain LLM(IChatModel llm,
        string inputKey = "prompt", string outputKey = "text")
    {
        return new LLMChain(llm, inputKey, outputKey);
    }

    public static RetreiveDocumentsChain RetreiveDocuments(VectorStoreIndexWrapper index,
        string inputKey = "query", string outputKey = "documents")
    {
        return new RetreiveDocumentsChain(index, inputKey, outputKey);
    }

    public static StuffDocumentsChain StuffDocuments(
        string inputKey = "documents", string outputKey = "combined")
    {
        return new StuffDocumentsChain(inputKey, outputKey);
    }

    public static UpdateMemoryChain UpdateMemory(BaseChatMemory memory,
        string requestKey = "query", string responseKey = "text")
    {
        return new UpdateMemoryChain(memory, requestKey, responseKey);
    }

    public static TTSChain<T> TTS<T>(ITextToSpeechModel<T> model,
        T settings,
        string inputKey = "text", string outputKey = "audio")
    {
        return new TTSChain<T>(model, settings, inputKey, outputKey);
    }

    public static STTChain<T> STT<T>(ISpeechToTextModel<T> model,
        T settings,
        string inputKey = "audio", string outputKey = "text")
    {
        return new STTChain<T>(model, settings, inputKey, outputKey);
    }
}