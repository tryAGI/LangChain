using LangChain.Cli.Commands;

namespace LangChain.Cli.IntegrationTests;

[Explicit]
[TestFixture]
public class CliTests
{
    [Test]
    public async Task DoCommand_WithFilesystemTool_ShouldReturnValidOutput()
    {
        await ("do " +
            "--tools filesystem " +
            "--directories \"/Users/havendv/GitHub/tryAGI/\" " +
            "--format markdown " +
            "--input \"Please show me path of FixOpenAPISpec project Program.cs which contains `openApiDocument.Paths` modification. Start from `/Users/havendv/GitHub/tryAGI/` dir. \"")
            .ShouldWork<DoCommand>();
    }
    
    [Test]
    public async Task DoCommand_WithConventionalCommitFormat_ShouldReturnValidOutput()
    {
        await ("do " +
               "--format conventional-commit " +
               "--input \"There was fixed a bug in FixOpenAPISpec project. Please show me the commit message.\"")
            .ShouldWork<DoCommand>();
    }
}