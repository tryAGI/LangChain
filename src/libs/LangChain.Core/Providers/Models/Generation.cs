namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="Text"></param>
public readonly record struct Generation(
    string Text)
{
    /// <summary>
    /// 
    /// </summary>
    public static Generation Empty { get; } = new(
        Text: string.Empty);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Generation operator +(Generation a, Generation b)
    {
        return new Generation(Text: a.Text + Environment.NewLine + b.Text);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Generation Add(Generation left, Generation right)
    {
        return left + right;
    }
}