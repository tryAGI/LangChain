using LangChain.Base;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Providers;

namespace LangChain.Chains.ConversationalRetrieval;

/// <inheritdoc/>
public class BaseConversationalRetrievalChainInput(
    BaseCombineDocumentsChain combineDocsChain,
    ILlmChain questionGenerator)
    : ChainInputs
{
    /// <summary>
    /// The chain used to combine any retrieved documents.
    /// </summary>
    public BaseCombineDocumentsChain CombineDocsChain { get; } = combineDocsChain;

    /// <summary>
    /// The chain used to generate a new question for the sake of retrieval.
    ///
    /// This chain will take in the current question (with variable `question`)
    /// and any chat history (with variable `chat_history`) and will produce
    /// a new standalone question to be used later on.
    /// </summary>
    public ILlmChain QuestionGenerator { get; } = questionGenerator;

    /// <summary>
    /// The output key to return the final answer of this chain in.
    /// </summary>
    public string OutputKey { get; set; } = "answer";

    /// <summary>
    /// Whether or not to pass the new generated question to the combine_docs_chain.
    ///
    /// If True, will pass the new generated question along.
    /// If False, will only use the new generated question for retrieval and pass the
    /// original question along to the <see cref="CombineDocsChain"/>.
    /// </summary>
    public bool RephraseQuestion { get; set; } = true;

    /// <summary>
    /// Return the retrieved source documents as part of the final result.
    /// </summary>
    public bool ReturnSourceDocuments { get; set; }

    /// <summary>
    /// Return the generated question as part of the final result.
    /// </summary>
    public bool ReturnGeneratedQuestion { get; set; }

    /// <summary>
    /// An optional function to get a string of the chat history.
    /// If None is provided, will use a default.
    /// </summary>
    public Func<IReadOnlyList<Message>, string?> GetChatHistory { get; set; } =
        ChatTurnTypeHelper.GetChatHistory;

    /// <summary>
    /// If specified, the chain will return a fixed response if no docs 
    /// are found for the question.
    /// </summary>
    public string? ResponseIfNoDocsFound { get; set; }
}