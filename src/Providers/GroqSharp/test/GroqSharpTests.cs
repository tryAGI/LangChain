using LangChain.Providers.GroqSharp.Predefined;
using NUnit.Framework;

namespace LangChain.Providers.GroqSharp.Test;

[TestFixture]
[Explicit]
public partial class GroqSharpTest
{
    [Test]
    public async Task Chat()
    {

        var config = new GroqSharpConfiguration()
        {
            ApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY") ??
                throw new InconclusiveException("GROQ_API_KEY is not set."),
            ModelId = "llama3-70b-8192"
        };

        var provider = new GroqSharpProvider(config);
        var model = new Llama370b8192(provider);

        string answer = await model.GenerateAsync("Generate some random name:");

        Console.WriteLine(answer);
    }

}
