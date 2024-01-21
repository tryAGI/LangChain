using LangChain.Providers;
using LangChain.Providers.Anthropic;
using Microsoft.AspNetCore.Mvc;

namespace LangChain.Samples.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class AnthropicSampleController : ControllerBase
{
    private readonly AnthropicModel _anthropicModel;
    private readonly ILogger<AnthropicSampleController> _logger;

    public AnthropicSampleController(
        AnthropicModel anthropicModel,
        ILogger<AnthropicSampleController> logger)
    {
        _anthropicModel = anthropicModel;
        _logger = logger;
    }

    [HttpGet(Name = "GetAnthropicResponse")]
    public async Task<string> Get()
    {
        var response = await _anthropicModel.GenerateAsync("What is a good name for a company that sells colourful socks?");
        
        return response.LastMessageContent;
    }
}