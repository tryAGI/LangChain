using Microsoft.Extensions.AI;

namespace LangChain.Serve.OpenAI;

public class ServeOptions
{
    private readonly Dictionary<string, IChatClient> _chatClients = new();

    public ServeOptions RegisterModel(string id, IChatClient chatClient)
    {
        chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));

        _chatClients[id ?? throw new ArgumentNullException(nameof(id))] = chatClient;
        return this;
    }

    public IChatClient? GetChatClient(string modelName)
    {
        return _chatClients.GetValueOrDefault(modelName);
    }

    public List<string> ListModels()
    {
        return _chatClients.Keys.ToList();
    }

    public bool ModelExists(string modelName)
    {
        return _chatClients.ContainsKey(modelName);
    }
}
