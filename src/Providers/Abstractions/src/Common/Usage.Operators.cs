// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public readonly partial record struct Usage
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Usage operator +(Usage a, Usage b)
    {
        return new Usage(
            InputTokens: a.InputTokens + b.InputTokens,
            OutputTokens: a.OutputTokens + b.OutputTokens,
            Messages: a.Messages + b.Messages,
            Time: a.Time + b.Time,
            PriceInUsd: a.PriceInUsd + b.PriceInUsd);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Usage operator -(Usage a, Usage b)
    {
        return new Usage(
            InputTokens: a.InputTokens - b.InputTokens,
            OutputTokens: a.OutputTokens - b.OutputTokens,
            Messages: a.Messages - b.Messages,
            Time: a.Time - b.Time,
            PriceInUsd: a.PriceInUsd - b.PriceInUsd);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Usage operator *(Usage a, double b)
    {
        return new Usage(
            InputTokens: (int)Math.Ceiling(a.InputTokens * b),
            OutputTokens: (int)Math.Ceiling(a.OutputTokens * b),
            Messages: a.Messages,
#if NET6_0_OR_GREATER
            Time: a.Time * b,
#else
            Time: TimeSpan.FromTicks((long)(a.Time.Ticks * b)),
#endif
            PriceInUsd: a.PriceInUsd * b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Usage operator /(Usage a, double b)
    {
        return new Usage(
            InputTokens: (int)Math.Ceiling(a.InputTokens / b),
            OutputTokens: (int)Math.Ceiling(a.OutputTokens / b),
            Messages: a.Messages,
#if NET6_0_OR_GREATER
            Time: a.Time / b,
#else
            Time: TimeSpan.FromTicks((long)(a.Time.Ticks / b)),
#endif
            PriceInUsd: a.PriceInUsd / b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Usage Add(Usage left, Usage right)
    {
        return left + right;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Usage Multiply(Usage left, double right)
    {
        return left * right;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Usage Divide(Usage left, double right)
    {
        return left / right;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Usage Subtract(Usage left, Usage right)
    {
        return left - right;
    }
}