using LangChain.Memory;
using LangChain.Providers.OpenAI.Predefined;
using static LangChain.Chains.Chain;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
    throw new InvalidOperationException("OPENAI_API_KEY environment variable is not found.");
var model = new Gpt35TurboModel(apiKey);


// create simple template for conversation for AI to know what piece of text it is looking at
var template = @"
The following is a friendly conversation between a human and an AI.
{history}
Human: {input}
AI:";

// To have a conversation thar remembers previous messages we need to use memory.
// For memory to work properly we need to specify AI and Human prefixes.
// Since in our template we have "AI:" and "Human:" we need to specify them here. Pay attention to spaces after prefixes.
var conversationBufferMemory = new ConversationBufferMemory(new FileChatMessageHistory("messages.json"))
{
    AiPrefix = "AI: ",
    HumanPrefix = "Human: "
};

// build chain. Notice that we don't set input key here. It will be set in the loop
var chain =
    // load history. at first it will be empty, but UpdateMemory will update it every iteration
    LoadMemory(conversationBufferMemory, outputKey: "history")
    | Template(template)
    | LLM(model)
    // update memory with new request from Human and response from AI
    | UpdateMemory(conversationBufferMemory, requestKey: "input", responseKey: "text");

// run an endless loop of conversation
while (true)
{
    Console.Write("Human: ");
    var input = Console.ReadLine() ?? string.Empty;
    if (input == "exit")
        break;

    // build a new chain using previous chain but with new input every time
    var chatChain = Set(input, "input")
        | chain;

    // get response from AI
    var res = await chatChain.Run("text");


    Console.Write("AI: ");
    Console.WriteLine(res);
}