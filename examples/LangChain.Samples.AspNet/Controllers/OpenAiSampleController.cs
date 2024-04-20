using LangChain.Providers.OpenAI;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Constants;

namespace LangChain.Samples.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenAiSampleController : ControllerBase
{
    private readonly OpenAiProvider _openAi;
    private readonly ILogger<OpenAiSampleController> _logger;

    public OpenAiSampleController(
        OpenAiProvider openAi,
        ILogger<OpenAiSampleController> logger)
    {
        _openAi = openAi;
        _logger = logger;
    }

    [HttpGet(Name = "GetOpenAiResponse")]
    public async Task<string> Get()
    {
        var llm = new OpenAiChatModel(_openAi, id: ChatModels.Gpt35Turbo);
        var response = await llm.GenerateAsync("What is a good name for a company that sells colourful socks?");
        
        return response.LastMessageContent;
    }
}