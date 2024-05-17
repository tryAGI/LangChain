namespace LangChain.Providers.Groq.Predefined;

public class Llama38B(GroqProvider provider)
    : GroqChatModel(provider, id: "llama3-8b-8192");

public class Llama370B(GroqProvider provider)
    : GroqChatModel(provider, id: "llama3-70b-8192");

public class Mixtral8X7B(GroqProvider provider)
    : GroqChatModel(provider, id: "mixtral-8x7b-32768");

public class Gemma7B(GroqProvider provider)
    : GroqChatModel(provider, id: "gemma-7b-it");