using LangChain.Providers.OpenAI;
using LangChain.Serve.OpenAI;
using OpenAI;

var builder = WebApplication.CreateBuilder();

// 1. Add LangChainServe
builder.Services.AddLangChainServeOpenAi();

// 2. Create a model
var provider = new OpenAiProvider(Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "");
var model = new OpenAiChatModel(provider, CreateChatCompletionRequestModel.Gpt4Turbo);

// 4. Optional. Add swagger to be able to test the API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Configure LangChainServe
app.UseLangChainServeOpenAi(options =>
{
    options.RegisterModel(model);
});

// 6. Optional. Add swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My OpenAI API V1");
});
app.Run();