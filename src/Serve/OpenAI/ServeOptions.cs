using LangChain.Providers;

namespace LangChain.Serve.OpenAI;

public class ServeOptions
{
    private readonly Dictionary<string, ChatModel> _models = new();

    public ServeOptions RegisterModel(ChatModel chatModel, string? overrideId = null)
    {
        chatModel = chatModel ?? throw new ArgumentNullException(nameof(chatModel));
        
        _models[overrideId ?? chatModel.Id] = chatModel;
        return this;
    }

    public ChatModel GetModel(string modelName)
    {
        return _models[modelName];
    }

    public List<string> ListModels()
    {
        return _models.Keys.ToList();
    }

    public bool ModelExists(string modelName)
    {
        return _models.ContainsKey(modelName);
    }
}