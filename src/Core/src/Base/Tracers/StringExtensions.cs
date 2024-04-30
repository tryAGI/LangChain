namespace LangChain.Base.Tracers;

/// <summary>
/// 
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static string Capitalize(this string word)
    {
        word = word ?? throw new ArgumentNullException(nameof(word));

        if (word.Length == 1)
        {
            return word.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
        }

        return word.Substring(0, 1).ToUpper(System.Globalization.CultureInfo.CurrentCulture)
               + word.Substring(1).ToLower(System.Globalization.CultureInfo.CurrentCulture);
    }
}