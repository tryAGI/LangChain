using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace LangChain.Samples.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenAiSampleController : ControllerBase
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<OpenAiSampleController> _logger;

    public OpenAiSampleController(
        IChatClient chatClient,
        ILogger<OpenAiSampleController> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    [HttpGet(Name = "GetOpenAiResponse")]
    public async Task<string> Get()
    {
        var response = await _chatClient.GetResponseAsync("What is a good name for a company that sells colourful socks?");

        return response.Text;
    }
}
