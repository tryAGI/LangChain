using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Serve;

public class ServeOptions
{
    private readonly Dictionary<string, Func<List<StoredMessage>, Task<StoredMessage>>> _models = new();

    public ServeOptions RegisterModel(string name, Func<List<StoredMessage>, Task<StoredMessage>> messageProcessor)
    {
        _models[name] = messageProcessor;
        return this;
    }

    public Func<List<StoredMessage>, Task<StoredMessage>> GetModel(string modelName)
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