using System.Text.Json;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.Prompts;

public abstract class BaseMessagePromptTemplate
{
    public abstract IReadOnlyList<string> InputVariables { get; }

    public abstract Task<List<Message>> FormatMessages(InputValues values);

    public SerializedMessagePromptTemplate Serialize()
    {
        var serialized = new SerializedMessagePromptTemplate
        {
            Type = this.GetType().Name,
            // You need to serialize 'this' to a JSON string, then deserialize it back to a dictionary
            // to mimic the JavaScript `JSON.parse(JSON.stringify(this))` behavior.
            AdditionalProperties = JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(this)) ?? new Dictionary<string, object>(),
        };
        return serialized;
    }
}