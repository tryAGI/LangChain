using System.Text.Json;
using LangChain.Chat;
using LangChain.Schema;

namespace LangChain.Prompts;

public abstract class BaseMessagePromptTemplate
{
    public abstract List<string> InputVariables { get; }

    public abstract Task<List<BaseChatMessage>> FormatMessages(InputValues values);

    public SerializedMessagePromptTemplate Serialize()
    {
        var serialized = new SerializedMessagePromptTemplate
        {
            Type = this.GetType().Name,
            // You need to serialize 'this' to a JSON string, then deserialize it back to a dictionary
            // to mimic the JavaScript `JSON.parse(JSON.stringify(this))` behavior.
            AdditionalProperties = JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(this))
        };
        return serialized;
    }
}