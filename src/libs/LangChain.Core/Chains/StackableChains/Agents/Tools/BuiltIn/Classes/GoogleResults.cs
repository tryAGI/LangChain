

namespace LangChain.Chains.StackableChains.Agents.Tools.BuiltIn.Classes;

public class GoogleResult
{
    public class Item
    {
        public string title { get; set; } = string.Empty;
        public string link { get; set; } = string.Empty;
        public string snippet { get; set; } = string.Empty;
        
    }

    public List<Item> items { get; set; } = [];
}