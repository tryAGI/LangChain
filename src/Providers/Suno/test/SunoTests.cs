using System.Diagnostics;

namespace LangChain.Providers.Suno.Tests;

[TestFixture]
[Explicit]
public class SunoTests
{
    [Test]
    public async Task Text2Music()
    {
        using var httpClient = new HttpClient();
        var provider = new SunoProvider(
            apiKey: Environment.GetEnvironmentVariable("SUNO_API_KEY") ??
            throw new InconclusiveException("SUNO_API_KEY is not set."),
            httpClient);
        var model = new SunoModel(provider);

        var data = await model.GenerateMusicAsync("Robots will soon rule the world");

        data.Should().NotBeNull();
        data.Images.Should().NotBeNullOrEmpty();
        data.Images.First().Should().NotBeNull();
        var bytes = data.Images.First().ToByteArray();
        bytes.Should().NotBeNullOrEmpty();
        
        await File.WriteAllBytesAsync("robots-must-rule.mp3", bytes);
        
        Process.Start("robots-must-rule.mp3");
    }
}