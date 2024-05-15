namespace LangChain.Providers.GroqSharp.Predefined
{

    public class Llama38b8192(GroqSharpProvider provider)
    : GroqSharpChatModel(provider, id: "llama3-8b-8192");

    public class Llama370b8192(GroqSharpProvider provider)
    : GroqSharpChatModel(provider, id: "llama3-70b-8192");

    public class Mixtral8x7b32768(GroqSharpProvider provider)
    : GroqSharpChatModel(provider, id: "mixtral-8x7b-32768");

    public class Gemma7bIt(GroqSharpProvider provider)
    : GroqSharpChatModel(provider, id: "gemma-7b-it");
    
}
