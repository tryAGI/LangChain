using System.Text;

namespace LangChain.Utilities.PostgresDatabases;

internal static class StringBuilderExtensions
{
    public static void AppendJoin(this StringBuilder builder, string separator, IEnumerable<string> collection)
    {
        var first = true;
        foreach (var line in collection)
        {
            if (first)
            {
                first = false;
                builder.Append(line);
            }
            else
            {
                builder.Append(separator);
                builder.Append(line);
            }
        }
    }
}