namespace LangChain.Utilities.Sql;

internal static class StringHelperer
{
    /// <summary>
    /// Truncate a string to a certain number of words, based on the max string length.
    /// </summary>
    public static string? TruncateWord(object content, int length, string suffix = "...")
    {
        if (length > 0 && content is string contentString)
        {
            if (contentString.Length <= length)
            {
                return contentString;
            }

            var substring = contentString.Substring(0, length - suffix.Length);

            return substring.Substring(0, substring.LastIndexOf(" ", StringComparison.Ordinal)) + suffix;
        }

        return content.ToString();
    }
}