namespace LangChain.Base.Tracers;

public static class StringExtensions
{
    public static string Capitalize(this string? word)
    {
        if (word == null)
        {
            return word;
        }

        if (word.Length == 1)
        {
            return word.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
        }

        return word.Substring(0, 1).ToUpper(System.Globalization.CultureInfo.CurrentCulture)
               + word.Substring(1).ToLower(System.Globalization.CultureInfo.CurrentCulture);
    }
}