using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Common;
using LangChain.Docstore;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Chains.ConversationalRetrieval;

/// <summary>
/// Chain for chatting with an index.
/// </summary>
public abstract class BaseConversationalRetrievalChain(BaseConversationalRetrievalChainInput fields) : BaseChain(fields)
{
    /// <summary> Chain input fields </summary>
    private readonly BaseConversationalRetrievalChainInput _fields = fields;

    public override IReadOnlyList<string> InputKeys => new[] { "question", "chat_history" };

    public override IReadOnlyList<string> OutputKeys
    {
        get
        {
            var outputKeys = new List<string> { _fields.OutputKey };
            if (_fields.ReturnSourceDocuments)
            {
                outputKeys.Add("source_documents");
            }

            if (_fields.ReturnGeneratedQuestion)
            {
                outputKeys.Add("generated_question");
            }

            return outputKeys.ToArray();
        }
    }

    protected override async Task<IChainValues> CallAsync(
        IChainValues values,
        CallbackManagerForChainRun? runManager)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        runManager ??= BaseRunManager.GetNoopManager<CallbackManagerForChainRun>();

        var question = values.Value["question"].ToString();

        var getChatHistory = _fields.GetChatHistory;
        var chatHistoryStr = getChatHistory(values.Value["chat_history"] as List<Message>);

        string? newQuestion;
        if (chatHistoryStr != null)
        {
            var callbacks = runManager.GetChild();
            newQuestion = await _fields.QuestionGenerator.Run(
                new Dictionary<string, object>
                {
                    ["question"] = question ?? string.Empty,
                    ["chat_history"] = chatHistoryStr
                },
                callbacks: new ManagerCallbacks(callbacks)).ConfigureAwait(false);
        }
        else
        {
            newQuestion = question;
        }

        var docs = await GetDocsAsync(newQuestion, values.Value).ConfigureAwait(false);
        var newInputs = new Dictionary<string, object>
        {
            ["chat_history"] = chatHistoryStr ?? string.Empty,
            ["input_documents"] = docs
        };

        if (_fields.RephraseQuestion)
        {
            newInputs["question"] = newQuestion;
        }

        newInputs.TryAddKeyValues(values.Value);

        var answer = await _fields.CombineDocsChain.Run(
            input: newInputs,
            callbacks: new ManagerCallbacks(runManager.GetChild())).ConfigureAwait(false);

        var output = new Dictionary<string, object>
        {
            [_fields.OutputKey] = answer
        };

        if (_fields.ReturnSourceDocuments)
        {
            output["source_documents"] = docs;
        }

        if (_fields.ReturnGeneratedQuestion)
        {
            output["generated_question"] = newQuestion;
        }

        return new ChainValues(output);
    }

    /// <summary>
    /// Get docs.
    /// </summary>
    /// <param name="question"></param>
    /// <param name="inputs"></param>
    /// <param name="runManager"></param>
    /// <returns></returns>
    protected abstract Task<List<Document>> GetDocsAsync(
        string question,
        Dictionary<string, object> inputs,
        CallbackManagerForChainRun? runManager = null);
}