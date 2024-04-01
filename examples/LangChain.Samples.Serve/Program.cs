using LangChain.Memory;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using LangChain.Serve;
using LangChain.Serve.Classes.Repository;
using static LangChain.Chains.Chain;
using Message = LangChain.Providers.Message;


var builder = WebApplication.CreateBuilder();



// 1. Add LangChainServe
builder.Services.AddLangChainServe();

// 2. Create a model
var model = new OllamaChatModel(new OllamaProvider(options: new OllamaOptions
{
    Temperature = 0,
    Stop = ["User:"],
}),"mistral:latest");


// 3. Optional. Add custom name generator
// After initiating conversation, this will generate a name for it
// If skipped, the conversation will be stored with current datetime as a name
builder.Services.AddCustomNameGenerator(async messages =>
{
    var template =
        @"You will be given conversation between User and Assistant. Your task is to give name to this conversation using maximum 3 words
Conversation:
{chat_history}
Your name: ";
    var conversationBufferMemory = await ConvertToConversationBuffer(messages);
    var chain = LoadMemory(conversationBufferMemory, "chat_history")
                | Template(template)
                | LLM(model);
    
    return await chain.Run("text") ?? string.Empty;
});

// 4. Optional. Add swagger to be able to test the API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Configure LangChainServe
app.UseLangChainServe(options =>
{
    // Register a model. Can be called multiple times to register multiple models
    // this will receive entire conversation and should return a response
    options.RegisterModel("Test model", async (messages) =>
    {
        var template = @"You are helpful assistant. Keep your answers short.
{chat_history}
Assistant:";
        // Convert messages to use with conversation buffer memory
        var conversationBufferMemory = await ConvertToConversationBuffer(messages);
        var chain = LoadMemory(conversationBufferMemory, "chat_history")
                    | Template(template)
                    | LLM(model);

        // get response and send it as AI answer
        var response = await chain.Run("text");
        return new StoredMessage
        {
            Author = MessageAuthor.Ai,
            Content = response ?? string.Empty,
        };
    });
});

// 6. Optional. Add swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
app.Run();
return;

async Task<ConversationBufferMemory> ConvertToConversationBuffer(List<StoredMessage> list)
{
    var conversationBufferMemory = new ConversationBufferMemory
    {
        Formatter =
        {
            HumanPrefix = "User",
            AiPrefix = "Assistant",
        }
    };
    List<Message> converted = list
        .Select(x => new Message(x.Content, x.Author == MessageAuthor.User ? MessageRole.Human : MessageRole.Ai))
        .ToList();
    
    await conversationBufferMemory.ChatHistory.AddMessages(converted);
    
    return conversationBufferMemory;
}