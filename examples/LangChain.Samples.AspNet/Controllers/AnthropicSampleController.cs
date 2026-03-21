using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace LangChain.Samples.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class AnthropicSampleController : ControllerBase
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<AnthropicSampleController> _logger;

    public AnthropicSampleController(
        IChatClient chatClient,
        ILogger<AnthropicSampleController> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    [HttpGet(Name = "GetAnthropicResponse")]
    public async Task<string> Get()
    {
        var response = await _chatClient.GetResponseAsync("What is a good name for a company that sells colourful socks?");

        return response.Text;
    }
}
