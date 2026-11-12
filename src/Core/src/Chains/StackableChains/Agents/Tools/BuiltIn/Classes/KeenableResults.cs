using System.Text.Json.Serialization;

namespace LangChain.Chains.StackableChains.Agents.Tools.BuiltIn.Classes;

public class KeenableResult
{
    public class Item
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Link { get; set; } = string.Empty;

        [JsonPropertyName("snippet")]
        public string Snippet { get; set; } = string.Empty;
    }

    [JsonPropertyName("results")]
    public List<Item> Results { get; set; } = [];
}
