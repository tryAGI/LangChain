using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Schema;
using LangChain.Cli.Models;
using Microsoft.Extensions.AI;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using Octokit;
using Tool = LangChain.Cli.Models.Tool;

namespace LangChain.Cli.Commands;

internal sealed class DoCommandHandler : ICommandHandler
{
    public Option<string> InputOption { get; } = CommonOptions.Input;
    public Option<FileInfo?> InputFileOption { get; } = CommonOptions.InputFile;
    public Option<FileInfo?> OutputFileOption { get; } = CommonOptions.OutputFile;
    public Option<bool> DebugOption { get; } = CommonOptions.Debug;
    public Option<string> ModelOption { get; } = CommonOptions.Model;
    public Option<Provider> ProviderOption { get; } = CommonOptions.Provider;
    public Option<string[]> ToolsOption { get; } = new(
        aliases: ["--tools", "-t"],
        description: $"Tools you want to use - {string.Join(", ", Enum.GetNames<Tool>())}. " +
                     "You can specify toolsets using square brackets, e.g., github[issues].")
    {
        AllowMultipleArgumentsPerToken = true,
    };
    public Option<DirectoryInfo[]> DirectoriesOption { get; } = new(
        aliases: ["--directories", "-d"],
        getDefaultValue: () => [new DirectoryInfo(".")],
        description: "Directories you want to use for filesystem.");
    public Option<Format> FormatOption { get; } = new(
        aliases: ["--format", "-f"],
        getDefaultValue: () => Format.Text,
        description: "Format of answer.");

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var input = context.ParseResult.GetValueForOption(InputOption) ?? string.Empty;
        var inputPath = context.ParseResult.GetValueForOption(InputFileOption);
        var outputPath = context.ParseResult.GetValueForOption(OutputFileOption);
        var debug = context.ParseResult.GetValueForOption(DebugOption);
        var model = context.ParseResult.GetValueForOption(ModelOption);
        var provider = context.ParseResult.GetValueForOption(ProviderOption);
        var toolStrings = context.ParseResult.GetValueForOption(ToolsOption) ?? [];
        var directories = context.ParseResult.GetValueForOption(DirectoriesOption) ?? [];
        var format = context.ParseResult.GetValueForOption(FormatOption);

        // Parse tool strings into tools and toolsets
        var toolsWithToolsets = toolStrings.Select(ToolExtensions.ParseTool).ToArray();
        var tools = toolsWithToolsets.Select(t => t.Tool).Distinct().ToArray();

        // Create a dictionary to store toolsets for each tool
        var toolsetsByTool = toolsWithToolsets
            .Where(t => t.Toolsets != null)
            .GroupBy(t => t.Tool)
            .ToDictionary(g => g.Key, g => g.SelectMany(t => t.Toolsets!).ToArray());

        var inputText = await Helpers.ReadInputAsync(input, inputPath).ConfigureAwait(false);
        var llm = Helpers.GetChatModel(model, provider, debug);

        var clients = await Task.WhenAll(tools.Select(async tool =>
        {
            // Get toolsets for this tool if any
            var toolsets = toolsetsByTool.GetValueOrDefault(tool) ?? [];

            return await McpClientFactory.CreateAsync(
                tool switch
                {
                    Tool.Filesystem => new McpServerConfig
                    {
                        Id = Tool.Filesystem.ToString(),
                        Name = Tool.Filesystem.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "npx",
                            ["arguments"] = $"-y @modelcontextprotocol/server-filesystem {string.Join(' ', directories.Select(x => x.FullName))}",
                        },
                    },
                    Tool.Fetch => new McpServerConfig
                    {
                        Id = Tool.Fetch.ToString(),
                        Name = Tool.Fetch.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "docker",
                            ["arguments"] = "run -i --rm mcp/fetch",
                        },
                    },
                    Tool.GitHub => new McpServerConfig
                    {
                        Id = Tool.GitHub.ToString(),
                        Name = Tool.GitHub.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "docker",
                            ["arguments"] = $"run -i --rm " +
                                            $"-e GITHUB_PERSONAL_ACCESS_TOKEN={Environment.GetEnvironmentVariable("GITHUB_TOKEN")} " +
                                            // $"-e GITHUB_DYNAMIC_TOOLSETS=1 " +
                                            (toolsets.Length != 0 ? 
                                                $"-e GITHUB_TOOLSETS={string.Join(',', toolsets)} "
                                                : string.Empty) +
                                            "ghcr.io/github/github-mcp-server",
                        },
                    },
                    Tool.Git => new McpServerConfig
                    {
                        Id = Tool.Git.ToString(),
                        Name = Tool.Git.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "docker",
                            ["arguments"] = $"run -i --rm {string.Join(' ', directories.Select(x => $"--mount type=bind,src={x},dst={x} "))} mcp/git",
                        },
                    },
                    Tool.Puppeteer => new McpServerConfig
                    {
                        Id = Tool.Puppeteer.ToString(),
                        Name = Tool.Puppeteer.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "docker",
                            ["arguments"] = "run -i --rm --init -e DOCKER_CONTAINER=true mcp/puppeteer",
                        },
                    },
                    Tool.SequentialThinking => new McpServerConfig
                    {
                        Id = Tool.SequentialThinking.ToString(),
                        Name = Tool.SequentialThinking.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "docker",
                            ["arguments"] = "run -i --rm mcp/sequentialthinking",
                        },
                    },
                    Tool.Slack => new McpServerConfig
                    {
                        Id = Tool.Slack.ToString(),
                        Name = Tool.Slack.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "docker",
                            ["arguments"] = $"run -i --rm -e SLACK_BOT_TOKEN={Environment.GetEnvironmentVariable("SLACK_BOT_TOKEN")} -e SLACK_TEAM_ID={Environment.GetEnvironmentVariable("SLACK_TEAM_ID")} -e SLACK_CHANNEL_IDS={Environment.GetEnvironmentVariable("SLACK_CHANNEL_IDS")} mcp/slack",
                        },
                    },
                    Tool.Figma => new McpServerConfig
                    {
                        Id = Tool.Figma.ToString(),
                        Name = Tool.Figma.ToString(),
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "npx",
                            ["arguments"] = $"-y figma-developer-mcp --figma-api-key={Environment.GetEnvironmentVariable("FIGMA_API_KEY")} --stdio",
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
                    },
                    InitializationTimeout = TimeSpan.FromMinutes(10),
                }).ConfigureAwait(false);
        })).ConfigureAwait(false);

        var aiTools = await Task.WhenAll(clients
            .Select(async client => await client.ListToolsAsync().ConfigureAwait(false)))
            .ConfigureAwait(false);

        Debug.WriteLine($"Found {aiTools.Length} AI functions.");
        foreach (var aiTool in aiTools.SelectMany(x => x))
        {
            Debug.WriteLine($"  -- {aiTool.Name}");
            Debug.WriteLine($"  {aiTool.Description}");
        }

        var response = await llm.GetResponseAsync(
            inputText,
            new ChatOptions
            {
                Tools = [
                    .. aiTools.SelectMany(x => x).ToArray(),
                    .. tools.Contains(Tool.Filesystem)
                        ? new [] { AIFunctionFactory.Create(
                            FindFilePathsByContent,
                            name: "FindFilePathsByContent",
                            description: "Finds file paths by content.") }
                        : [],
                    .. tools.Contains(Tool.GitHub)
                        ? new [] { AIFunctionFactory.Create(
                            GetAvailableLabels,
                            name: "GetAvailableLabelsForRepository",
                            description: "Retrieves all available labels for a GitHub repository.") }
                        : [],
                ],
                ResponseFormat = format switch
                {
                    Format.Text => ChatResponseFormat.Text,
                    Format.Lines => ChatResponseFormatForType<StringArraySchema>(),
                    Format.ConventionalCommit => ChatResponseFormatForType<ConventionalCommitSchema>(
                        schemaName: "ConventionalCommitSchema",
                        schemaDescription: "Conventional commit schema. Use this schema to generate conventional commits."),
                    Format.Markdown => ChatResponseFormatForType<MarkdownSchema>(
                        schemaName: "MarkdownSchema",
                        schemaDescription: "Markdown schema. Use this schema to generate markdown."),
                    Format.Json => ChatResponseFormat.Json,
                    _ => throw new ArgumentException($"Unknown format: {format}"),
                },
            }).ConfigureAwait(false);

        var output = response.Text;
        if (format == Format.Lines)
        {
            var value = JsonSerializer.Deserialize<StringArraySchema>(response.Text);

            output = string.Join(Environment.NewLine, value?.Value ?? []);
        }
        else if (format == Format.Markdown)
        {
            var value = JsonSerializer.Deserialize<MarkdownSchema>(response.Text);

            output = value?.Markdown ?? string.Empty;
        }
        else if (format == Format.ConventionalCommit)
        {
            var value = JsonSerializer.Deserialize<ConventionalCommitSchema>(response.Text);

            output = value?.ToString() ?? string.Empty;
        }

        await Helpers.WriteOutputAsync(output, outputPath, context.Console).ConfigureAwait(false);

        return 0;

        [Description("Finds file paths by content.")]
        static async Task<IList<string>> FindFilePathsByContent(
            [Description("The directory in which the search will be performed. Includes all subdirectories")] string directory,
            [Description("The content to search for in the files. Ignores case.")] string content)
        {
            var paths = new List<string>();

            Debug.WriteLine($"Searching for files in \"{directory}\" containing \"{content}\"...");

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

            Debug.WriteLine($"Found {paths.Count} files:");
            foreach (var path in paths)
            {
                Debug.WriteLine(path);
            }

            return paths;
        }
        
        [Description("Retrieves all available labels for a GitHub repository.")]
        static async Task<IReadOnlyList<Label>> GetAvailableLabels(
            [Description("The owner of the repository")] string owner,
            [Description("The name of the repository")] string name)
        {
            var github = new GitHubClient(new ProductHeaderValue("LangChain-DO-MCP-extension"))
            {
                Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN") ??
                                              throw new InvalidOperationException("GITHUB_TOKEN environment variable is not set."))
            };
            var labels = await github.Issue.Labels.GetAllForRepository(owner, name).ConfigureAwait(false);
            
            return labels;
        }
    }

    public static ChatResponseFormatJson ChatResponseFormatForType<T>(
        string? schemaName = null,
        string? schemaDescription = null)
    {
        return ChatResponseFormat.ForJsonSchema(
            JsonSerializerOptions.Default.GetJsonSchemaAsNode(typeof(T), new JsonSchemaExporterOptions
            {
                // Marks root-level types as non-nullable
                TreatNullObliviousAsNonNullable = true,
            }).Deserialize<JsonElement>(), schemaName, schemaDescription);
    }

    public static ChatResponseFormatJson Markdown { get; } = ChatResponseFormatForType<MarkdownSchema>(
        schemaName: "MarkdownSchema",
        schemaDescription: "Markdown schema. Use this schema to generate markdown.");
}

#pragma warning disable CA1812

internal sealed class StringArraySchema
{
    public string[] Value { get; set; } = [];
}

internal sealed class MarkdownSchema
{
    public string Markdown { get; set; } = string.Empty;
}

/// <summary>
/// Represents a single commit following Conventional Commit spec
/// https://www.conventionalcommits.org/en/v1.0.0
/// </summary>
internal sealed class ConventionalCommitSchema
{
    /// <summary>
    /// Commit type (feat, fix, docs, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Optional commit scope (e.g., "api")
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// Indicates breaking change (using "!")
    /// </summary>
    public bool IsBreakingChange { get; set; }

    /// <summary>
    /// Short commit message description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    // /// <summary>
    // /// Optional detailed commit body
    // /// </summary>
    // public string? Body { get; set; }

    // // Optional footers (e.g., BREAKING CHANGE, Refs, Reviewed-by)
    // public Dictionary<string, string> Footers { get; set; } = [];

    /// <summary>
    /// Generate conventional commit formatted string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var scopeText = string.IsNullOrWhiteSpace(Scope) ? "" : $"({Scope})";
        var breaking = IsBreakingChange ? "!" : "";

        var header = $"{Type}{scopeText}{breaking}: {Description}";

        var commitBuilder = new StringBuilder(header);

        // if (!string.IsNullOrWhiteSpace(Body))
        // {
        //     commitBuilder.AppendLine().AppendLine().Append(Body);
        // }
        //
        // if (Footers.Count > 0)
        // {
        //     commitBuilder.AppendLine();
        //     foreach (var footer in Footers)
        //     {
        //         commitBuilder.AppendLine().Append($"{footer.Key}: {footer.Value}");
        //     }
        // }

        return commitBuilder.ToString();
    }
}
