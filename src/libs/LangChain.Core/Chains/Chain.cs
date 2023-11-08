using LangChain.Abstractions.Chains.Base;
using LangChain.Chains.HelperChains;
using LangChain.Indexes;
using LangChain.Providers;

namespace LangChain.Chains;

public static class Chain
{
    public static BaseStackableChain Template(string template,
        string outputKey = "prompt")
    {
        return new PromptChain(template, outputKey);
    }

    public static BaseStackableChain Set(string value, string outputKey = "value")
    {
        return new SetChain(value, outputKey);
    }

    public static BaseStackableChain LLM(IChatModel llm, 
        string inputKey = "prompt", string outputKey = "text")
    {
        return new LLMChain(llm, inputKey, outputKey);
    }

    public static BaseStackableChain RetreiveDocuments(VectorStoreIndexWrapper index, 
        string inputKey = "query", string outputKey = "documents")
    {
        return new RetreiveDocumentsChain(index, inputKey, outputKey);
    }

    public static BaseStackableChain StuffDocuments(
        string inputKey = "documents", string outputKey = "combined")
    {
        return new StuffDocumentsChain(inputKey, outputKey);
    }
}