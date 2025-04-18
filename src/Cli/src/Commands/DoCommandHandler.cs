using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Schema;
using Microsoft.Extensions.AI;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;

namespace LangChain.Cli.Commands;

internal sealed class DoCommandHandler : ICommandHandler
{
    public Option<string> InputOption { get; } = CommonOptions.Input;
    public Option<string> InputFileOption { get; } = CommonOptions.InputFile;
    public Option<string> OutputFileOption { get; } = CommonOptions.OutputFile;
    public Option<bool> DebugOption { get; } = CommonOptions.Debug;
    public Option<string> ModelOption { get; } = CommonOptions.Model;
    public Option<string> ProviderOption { get; } = CommonOptions.Provider;
    public Option<string[]> ToolsOption { get; } = new(
        aliases: ["--tools", "-t"],
        parseArgument: result => result.Tokens.SelectMany(t => t.Value.Split(',')).ToArray(),
        description: $"Tools you want to use. Example: --tools={string.Join(",", Formats.All)}");
    public Option<string[]> DirectoriesOption { get; } = new(
        aliases: ["--directories", "-d"],
        getDefaultValue: () => ["."],
        description: "Directories you want to use for filesystem.");
    public Option<string> FormatOption { get; } = new(
        aliases: ["--format", "-f"],
        getDefaultValue: () => Formats.Text,
        description: $"Format of answer. Can be {string.Join(" or ", Formats.All)}.");

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var input = context.ParseResult.GetValueForOption(InputOption) ?? string.Empty;
        var inputPath = context.ParseResult.GetValueForOption(InputFileOption) ?? string.Empty;
        var outputPath = context.ParseResult.GetValueForOption(OutputFileOption);
        var debug = context.ParseResult.GetValueForOption(DebugOption);
        var model = context.ParseResult.GetValueForOption(ModelOption);
        var provider = context.ParseResult.GetValueForOption(ProviderOption);
        var tools = context.ParseResult.GetValueForOption(ToolsOption) ?? [];
        var directories = context.ParseResult.GetValueForOption(DirectoriesOption) ?? [];
        var format = context.ParseResult.GetValueForOption(FormatOption);

        var inputText = await Helpers.ReadInputAsync(input, inputPath).ConfigureAwait(false);
        var llm = Helpers.GetChatModel(model, provider, debug);

        var clients = await Task.WhenAll(tools.Select(async tool =>
        {
            return await McpClientFactory.CreateAsync(
                tool switch
                {
                    Tools.Filesystem => new McpServerConfig
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
                    Tools.Fetch => new McpServerConfig
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
                    Tools.GitHub => new McpServerConfig
                    {
                        Id = "Fetch",
                        Name = "Fetch",
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new Dictionary<string, string>
                        {
                            ["command"] = "docker",
                            ["arguments"] = $"run -i --rm -e GITHUB_PERSONAL_ACCESS_TOKEN={Environment.GetEnvironmentVariable("GITHUB_TOKEN")} ghcr.io/github/github-mcp-server",
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
                    .. tools.Contains("filesystem")
                        ? new [] { AIFunctionFactory.Create(
                            FindFilePathsByContent,
                            name: "FindFilePathsByContent",
                            description: "Finds file paths by content.") }
                        : [],
                ],
                ResponseFormat = format switch
                {
                    Formats.Text => ChatResponseFormat.Text,
                    Formats.Lines => ChatResponseFormatForType<StringArraySchema>(),
                    Formats.ConventionalCommit => ChatResponseFormatForType<ConventionalCommitSchema>(
                        schemaName: "ConventionalCommitSchema",
                        schemaDescription: "Conventional commit schema. Use this schema to generate conventional commits."),
                    Formats.Markdown => ChatResponseFormatForType<MarkdownSchema>(
                        schemaName: "MarkdownSchema",
                        schemaDescription: "Markdown schema. Use this schema to generate markdown."),
                    Formats.Json => ChatResponseFormat.Json,
                    _ => throw new ArgumentException($"Unknown format: {format}"),
                },
            }).ConfigureAwait(false);

        var output = response.Text;
        if (format == Formats.Lines)
        {
            var value = JsonSerializer.Deserialize<StringArraySchema>(response.Text);

            output = string.Join(Environment.NewLine, value?.Value ?? []);
        }
        else if (format == Formats.Markdown)
        {
            var value = JsonSerializer.Deserialize<MarkdownSchema>(response.Text);

            output = value?.Markdown ?? string.Empty;
        }
        else if (format == Formats.ConventionalCommit)
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
