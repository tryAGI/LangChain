using LangChain.Serve.OpenAI;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder();

// 1. Add LangChainServe
builder.Services.AddLangChainServeOpenAi();

// 2. Create a model via MEAI IChatClient
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
var openAiClient = new OpenAIClient(apiKey);
IChatClient chatClient = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();

// 3. Optional. Add swagger to be able to test the API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Configure LangChainServe with IChatClient
app.UseLangChainServeOpenAi(options =>
{
    options.RegisterModel("gpt-4o-mini", chatClient);
});

// 5. Optional. Add swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My OpenAI API V1");
});
app.Run();
