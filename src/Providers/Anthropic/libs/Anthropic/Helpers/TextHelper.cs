namespace LangChain.Providers.Anthropic.Helpers;

public static class TextHelper
{
    public static string GetTextBetweenDelimiters(string str, string startText, string endText)
    {
        str = str ?? throw new ArgumentNullException(nameof(str));
        startText = startText ?? throw new ArgumentNullException(nameof(startText));

        var ind = str.IndexOf(startText, StringComparison.CurrentCultureIgnoreCase);
        if (ind != -1)
            str = str.Substring(ind + startText.Length);
        ind = str.IndexOf(endText, StringComparison.CurrentCultureIgnoreCase);
        if (ind != -1)
            str = str.Remove(ind);
        return str;
    }
}