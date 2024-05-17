using LangChain.Providers.Groq.Predefined;

namespace LangChain.Providers.Groq.Test;

[TestFixture]
[Explicit]
public partial class GroqTests
{
    [Test]
    public async Task Chat()
    {
        var config = new GroqConfiguration()
        {
            ApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY") ??
                throw new InconclusiveException("GROQ_API_KEY is not set.")
        };

        var provider = new GroqProvider(config);
        var model = new Llama370B(provider);

        string answer = await model.GenerateAsync("Generate some random name:");

        answer.Should().NotBeNull();

        Console.WriteLine(answer);
    }
}
