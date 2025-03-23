using System.CommandLine;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;

namespace LangChain.Cli.Commands;

internal sealed class DoCommand : Command
{
    public DoCommand() : base(name: "do", description: "Generates text using a prompt.")
    {
        var inputOption = CommonOptions.Input;
        var inputFileOption = CommonOptions.InputFile;
        var outputFileOption = CommonOptions.OutputFile;
        var toolsOption = new Option<string[]>(
            aliases: ["--tools", "-t"],
            parseArgument: result => [.. result.Tokens.SelectMany(t => t.Value.Split(','))],
            description: "Tools you want to use. Example: --tools=filesystem,fetch");
        var directoriesOption = new Option<string[]>(
            aliases: ["--directories", "-d"],
            getDefaultValue: () => ["."],
            description: "Directories you want to use.");
        AddOption(inputOption);
        AddOption(inputFileOption);
        AddOption(outputFileOption);
        AddOption(toolsOption);
        AddOption(directoriesOption);

        this.SetHandler(HandleAsync, inputOption, inputFileOption, outputFileOption, toolsOption, directoriesOption);
    }

    private static async Task HandleAsync(string input, string inputPath, string outputPath, string[] tools, string[] directories)
    {
        var inputText = await Helpers.ReadInputAsync(input, inputPath).ConfigureAwait(false);
        var llm = await Helpers.GetChatModelAsync().ConfigureAwait(false);
        // llm.RequestSent += (_, request) => Console.WriteLine($"RequestSent: {request.Messages.AsHistory()}");
        // llm.ResponseReceived += (_, response) => Console.WriteLine($"ResponseReceived: {response}");

        // var fileSystemService = new FileSystemService();
        // llm.AddGlobalTools(fileSystemService.AsTools(), fileSystemService.AsCalls());
        //
        var clients = await Task.WhenAll(tools.Select(async tool =>
        {
            return await McpClientFactory.CreateAsync(
                tool switch
                {
                    "filesystem" => new McpServerConfig
                    {
                        Id = "Filesystem",
                        Name = "Filesystem",
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "npx",
                            ["arguments"] = $"-y @modelcontextprotocol/server-filesystem {string.Join(' ', directories)}",
                        },
                    },
                    "fetch" => new McpServerConfig
                    {
                        Id = "Fetch",
                        Name = "Fetch",
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "python",
                            ["arguments"] = "-m mcp_server_fetch",
                        },
                    },
                    _ => throw new ArgumentException($"Unknown tool: {tool}"),
                },
                new McpClientOptions
                {
                    ClientInfo = new Implementation
                    {
                        Name = "LangChain CLI DO client",
                        Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0",
                    }
                }).ConfigureAwait(false);
        })).ConfigureAwait(false);
        
        var aiTools = await Task.WhenAll(clients
            .Select(async client => await client.GetAIFunctionsAsync().ConfigureAwait(false)))
            .ConfigureAwait(false);
        
        Console.WriteLine($"Found {aiTools.Length} AI functions.");
        foreach (var aiTool in aiTools.SelectMany(x => x))
        {
            Console.WriteLine($"  -- {aiTool.Name}");
            Console.WriteLine($"  {aiTool.Description}");
        }

        // Call the chat client using the tools.
        var response = await llm.GetResponseAsync(
            inputText,
            new ChatOptions
            {
                Tools = [
                    .. aiTools.SelectMany(x => x).ToArray(),
                    .. tools.Contains("filesystem")
                        ? new [] { AIFunctionFactory.Create(
                            FindFilePathsByContent,
                            name: "FindFilePathsByContent",
                            description: "Finds file paths by content.") }
                        : [],
                ],
            }).ConfigureAwait(false);
        
        await Helpers.WriteOutputAsync(response.Text, outputPath).ConfigureAwait(false);
        
        return;
        
        [Description("Finds file paths by content.")]
        static async Task<IList<string>> FindFilePathsByContent(
            [Description("The directory in which the search will be performed. Includes all subdirectories")] string directory,
            [Description("The content to search for in the files. Ignores case.")] string content)
        {
            var paths = new List<string>();

            Console.WriteLine($"Searching for files in \"{directory}\" containing \"{content}\"...");

            foreach (var path in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    var extension = Path.GetExtension(path);
                    if (extension is not ".txt" and not ".md" and not ".json" and not ".cs" and not ".csproj" and not ".sln" and not ".sh" and not ".yml" and not ".yaml")
                    {
                        continue;
                    }

                    //FileInfo info = new FileInfo(path);
                    var text = await File.ReadAllTextAsync(path).ConfigureAwait(false);

                    if (text.Contains(content, StringComparison.OrdinalIgnoreCase))
                    {
                        paths.Add(path);
                    }
                }
#pragma warning disable CA1031
                catch (Exception)
#pragma warning restore CA1031
                {
                    // ignore
                }
            }

            Console.WriteLine($"Found {paths.Count} files:");
            foreach (var path in paths)
            {
                Console.WriteLine(path);
            }

            return paths;
        }
    }
}