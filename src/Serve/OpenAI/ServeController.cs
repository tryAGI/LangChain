using System.Text.Json.Serialization;
using OpenAI.Models;

namespace LangChain.Serve.OpenAI;

public class ServeController(
    ServeOptions options)
{
    public ModelsList GetModel()
    {
        return new ModelsList
        {
            Models = options.ListModels().Select(static x => new Model(x, ownedBy: "OpenAI")).ToList()
        };
    }

    public ModelsList ListModels()
    {
        return new ModelsList
        {
            Models = options.ListModels().Select(static x => new Model(x, ownedBy: "OpenAI")).ToList()
        };
    }
}

public sealed class ModelsList
{
    [JsonInclude]
    [JsonPropertyName("data")]
    public IReadOnlyCollection<Model> Models { get; set; } = [];
}