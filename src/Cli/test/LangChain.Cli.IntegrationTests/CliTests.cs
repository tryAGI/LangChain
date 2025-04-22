using LangChain.Cli.Commands;

namespace LangChain.Cli.IntegrationTests;

[Explicit]
[TestFixture]
public class CliTests
{
    [Test]
    public async Task DoCommand_WithHelp_ShouldReturnValidOutput()
    {
        await "do --help"
            .ShouldWork<DoCommand>();
    }
    
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
               "--format ConventionalCommit " +
               "--input \"There was fixed a bug in FixOpenAPISpec project. Please show me the commit message.\"")
            .ShouldWork<DoCommand>();
    }
    
    [Test]
    public async Task DoCommand_WithOpenRouterProvider_ShouldReturnValidOutput()
    {
        await ("do " +
               "--format ConventionalCommit " +
               "--provider openrouter " +
               "--model free-fast " +
               "--input \"Please show me the commit message for the following text: There was fixed a bug in FixOpenAPISpec project.\"")
            .ShouldWork<DoCommand>();
    }
    
    [Test]
    public async Task DoCommand_WithGitAndFileSystem_CreatesRepo_ShouldReturnValidOutput()
    {
        await ("do " +
               "--tools filesystem,git " +
               "--directories \"/Users/havendv/GitHub/tryAGI/\" " +
               "--provider openrouter " +
               "--model free-fast " +
               "--input \"Please create new repo with name `Do` in `/Users/havendv/GitHub/tryAGI/` dir and git init it.\"")
            .ShouldWork<DoCommand>();
    }
    
    [Test]
    public async Task DoCommand_AutoLabeling_ShouldReturnValidOutput()
    {
        await ("do " +
               "--tools github[issues] " +
               "--provider openrouter " +
               "--model free-fast " +
               "--debug " +
               "--input \"You work in the tryAGI/Replicate repository on the issue #97. Always retrieve issue body/comments and always retrieve available labels (because almost always there is custom labels), and always call `update_issue` tool to update the issue with suitable labels. Don't change body and other data.\"")
            .ShouldWork<DoCommand>();
    }
}