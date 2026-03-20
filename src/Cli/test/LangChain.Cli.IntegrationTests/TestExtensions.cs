using System.CommandLine;
using System.CommandLine.Parsing;

namespace LangChain.Cli.IntegrationTests;

public static class TestExtensions
{
    public static async Task ShouldWork<T>(this string arguments) where T : Command, new()
    {
        // Arrange
        var rootCommand = new RootCommand();
        rootCommand.Add(new T());

        // Act
        var parseResult = CommandLineParser.Parse(rootCommand, arguments, new ParserConfiguration());
        var result = await parseResult.InvokeAsync(new InvocationConfiguration(), CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }
}
