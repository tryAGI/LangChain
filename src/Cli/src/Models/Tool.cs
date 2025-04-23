namespace LangChain.Cli.Models;

internal enum Tool
{
    Filesystem,
    Fetch,
    GitHub,
    Git,
    Puppeteer,
    SequentialThinking,
    Slack,
    Figma,
    DocumentConversion,
}

// Extension class to handle tool parsing with optional toolsets
internal static class ToolExtensions
{
    // Parse a string into a Tool with optional toolset
    public static (Tool Tool, string[]? Toolsets) ParseTool(string input)
    {
        // Check if the input contains a toolset in square brackets
        int openBracketIndex = input.IndexOf('[', StringComparison.Ordinal);
        if (openBracketIndex > 0 && input.EndsWith(']'))
        {
            string toolName = input.Substring(0, openBracketIndex);
            string toolsetsString = input.Substring(openBracketIndex + 1, input.Length - openBracketIndex - 2);

            // Split by comma to handle multiple toolsets
            string[] toolsets = toolsetsString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (Enum.TryParse<Tool>(toolName, true, out var tool))
            {
                return (tool, toolsets);
            }
        }

        // No toolset specified, just parse the tool name
        if (Enum.TryParse<Tool>(input, ignoreCase: true, out var simpleTool))
        {
            return (simpleTool, null);
        }

        throw new ArgumentException($"Unknown tool: {input}");
    }
}