using System.CommandLine;
using System.CommandLine.Parsing;

namespace LangChain.Cli.IntegrationTests;

public static class TestExtensions
{
    public static async Task ShouldWork<T>(this string arguments) where T : Command, new()
    {
        // Arrange
        var outputWriter = new StringWriter();
        var errorWriter = new StringWriter();
        var rootCommand = new RootCommand();
        rootCommand.Add(new T());

        // Act
        var parseResult = CommandLineParser.Parse(rootCommand, arguments, new ParserConfiguration());
        var result = await parseResult.InvokeAsync(new InvocationConfiguration
        {
            Output = outputWriter,
            Error = errorWriter,
        }, CancellationToken.None);

        Console.WriteLine(errorWriter.ToString());
        Console.WriteLine(outputWriter.ToString());

        // Assert
        result.Should().Be(0);
        errorWriter.ToString().Trim().Should().Be(string.Empty);
        outputWriter.ToString().Trim().Should().NotBeNullOrEmpty();
    }
}
