namespace LangChain.Providers.Azure.Tests;

[TestFixture, Explicit]
public class GenerativeModelTests
{
    [Test]
    public async Task ShouldGenerateSuccessfully()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("AZURE_OPENAI_API_KEY is not set");
        var apiEndpoint =
            Environment.GetEnvironmentVariable("AZURE_OPENAI_API_ENDPOINT", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("AZURE_OPENAI_API_ENDPOINT is not set");
        var deploymentName =
            Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set");

        var configuration = new AzureOpenAiConfiguration
        {
            Id = deploymentName,
            ApiKey = apiKey,
            Endpoint = apiEndpoint,
        };
        var provider = new AzureOpenAiProvider(configuration);
        var model = new AzureOpenAiChatModel(provider, deploymentName);

        var result = await model.GenerateAsync(ChatRequest.ToChatRequest("Write a less 'poemy' Poem"));

        result.Messages.Count.Should().BeGreaterThan(1);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
    }
}