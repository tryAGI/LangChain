using System.CommandLine;
using System.CommandLine.IO;

namespace LangChain.Cli.IntegrationTests;

public static class TestExtensions
{
    public static async Task ShouldWork<T>(this string arguments) where T : Command, new()
    {
        // Arrange
        var console = new TestConsole();
        var rootCommand = new RootCommand
        {
            new T(),
        };
        
        //var test = rootCommand.Parse(arguments);
        //test.Errors.Should().BeEmpty();
        
        // Act
        var result = await rootCommand.InvokeAsync(arguments, console);

        Console.WriteLine(console.Error.ToString());
        Console.WriteLine(console.Out.ToString());
        
        // Assert
        result.Should().Be(0);
        console.Error.ToString()?.Trim().Should().Be(string.Empty);
        console.Out.ToString()?.Trim().Should().NotBeNullOrEmpty();
    }
}