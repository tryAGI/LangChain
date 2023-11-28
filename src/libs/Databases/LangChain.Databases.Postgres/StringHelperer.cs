namespace LangChain.Databases;

internal class StringHelperer
{
    /// <summary>
    /// Truncate a string to a certain number of words, based on the max string length.
    /// </summary>
    public static string? TruncateWord(object content, int length, string suffix = "...")
    {
        if (length > 0 && content is string contentString)
        {
            if (contentString.Length <= length)
                return contentString;


            var substring = contentString[..(length - suffix.Length)];

            return substring[..substring.LastIndexOf(" ", StringComparison.Ordinal)] + suffix;
        }

        return content.ToString();
    }
}