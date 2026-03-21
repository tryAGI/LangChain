using LangChain.Providers;
using Microsoft.Extensions.AI;

namespace LangChain.Serve.OpenAI;

public class ServeOptions
{
    private readonly Dictionary<string, ChatModel> _models = new();
    private readonly Dictionary<string, IChatClient> _chatClients = new();

    public ServeOptions RegisterModel(ChatModel chatModel, string? overrideId = null)
    {
        chatModel = chatModel ?? throw new ArgumentNullException(nameof(chatModel));

        _models[overrideId ?? chatModel.Id] = chatModel;
        return this;
    }

    public ServeOptions RegisterModel(string id, IChatClient chatClient)
    {
        chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));

        _chatClients[id ?? throw new ArgumentNullException(nameof(id))] = chatClient;
        return this;
    }

    public ChatModel GetModel(string modelName)
    {
        return _models[modelName];
    }

    public IChatClient? GetChatClient(string modelName)
    {
        return _chatClients.GetValueOrDefault(modelName);
    }

    public List<string> ListModels()
    {
        return _models.Keys.Union(_chatClients.Keys).ToList();
    }

    public bool ModelExists(string modelName)
    {
        return _models.ContainsKey(modelName) || _chatClients.ContainsKey(modelName);
    }
}
