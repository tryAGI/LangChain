namespace LangChain.Providers.Anthropic.Helpers;

public class TextHelper
{
    public static string GetTextBetweenDelimiters(string str, string startText, string endText)
    {
        var ind = str.ToLower().IndexOf(startText.ToLower());
        if (ind != -1)
            str = str.Substring(ind + startText.Length);
        ind = str.ToLower().IndexOf(endText.ToLower());
        if (ind != -1)
            str = str.Remove(ind);
        return str;
    }
}