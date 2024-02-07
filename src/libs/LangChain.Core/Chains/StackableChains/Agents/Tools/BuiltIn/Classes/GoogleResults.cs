

namespace LangChain.Chains.StackableChains.Agents.Tools.BuiltIn.Classes;

public class GoogleResult
{
    public class Item
    {
        public string title { get; set; }
        public string link { get; set; }
        public string snippet { get; set; }
        
    }
    public List<Item> items { get; set; }
}