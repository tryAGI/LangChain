using LangChain.Providers.OpenAI;
using Microsoft.AspNetCore.Mvc;

namespace LangChain.Samples.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenAiSampleController : ControllerBase
{
    private readonly OpenAiModel _openAi;
    private readonly ILogger<OpenAiSampleController> _logger;

    public OpenAiSampleController(
        OpenAiModel openAi,
        ILogger<OpenAiSampleController> logger)
    {
        _openAi = openAi;
        _logger = logger;
    }

    [HttpGet(Name = "GetOpenAiResponse")]
    public async Task<string> Get()
    {
        var response = await _openAi.GenerateAsync("What is a good name for a company that sells colourful socks?");
        
        return response.LastMessageContent;
    }
}