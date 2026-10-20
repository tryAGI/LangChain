using System.CommandLine;
using LangChain.Cli.Models;
using Microsoft.Extensions.AI;

namespace LangChain.Cli.IntegrationTests;

[TestFixture]
[NonParallelizable]
internal sealed class ModelPresetsTests
{
    [Test]
    public void ModelOption_UsesSupportedDefault()
    {
        var option = CommonOptions.Model;
        var command = new RootCommand { option };

        command.Parse([]).GetValue(option).Should().Be("gpt-5.6-sol");
    }

    [TestCase(null, Provider.OpenAi, "gpt-5.6-terra")]
    [TestCase("latest-fast", Provider.OpenAi, "gpt-5.6-terra")]
    [TestCase("latest-smart", Provider.OpenAi, "o3")]
    [TestCase("free", Provider.Free, "nvidia/nemotron-3-super-120b-a12b:free")]
    [TestCase("free-fast", Provider.Free, "nvidia/nemotron-3-super-120b-a12b:free")]
    [TestCase("free-fast", Provider.OpenRouter, "nvidia/nemotron-3-super-120b-a12b:free")]
    [TestCase("free-smart", Provider.Free, "nvidia/nemotron-3.5-super-vl:free")]
    [TestCase("custom-model", Provider.OpenAi, "custom-model")]
    public void ChatClient_ResolvesModelWithoutSendingRequests(string? model, Provider provider, string expectedModel)
    {
        var variable = Helpers.GetApiKeyEnvironmentVariable(provider);
        var originalValue = Environment.GetEnvironmentVariable(variable);
        try
        {
            Environment.SetEnvironmentVariable(variable, "offline-model-preset-test");
            using var client = Helpers.GetChatModel(model, provider);

            client.GetService<ChatClientMetadata>()!.DefaultModelId.Should().Be(expectedModel);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, originalValue);
        }
    }
}
