using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangChain.Schema;

[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(IDictionary<string, object>))]
internal sealed partial class LangChainJsonSerializerContext : JsonSerializerContext
{
    internal static LangChainJsonSerializerContext Metadata { get; } = CreateMetadataContext();

    private static LangChainJsonSerializerContext CreateMetadataContext()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new LangChainJsonObjectConverter());
        return new LangChainJsonSerializerContext(options);
    }
}
