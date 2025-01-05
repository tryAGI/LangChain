
using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Context;

/// <summary>
/// 
/// </summary>
public class ConsoleTraceHook : StackableChainHook
{
    /// <summary>
    /// 
    /// </summary>
    public bool UseColors { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public int ValuesLength { get; set; } = 40;

    /// <inheritdoc/>
    public override void ChainStart(StackableChainValues values)
    {
        Console.WriteLine();
    }

    /// <inheritdoc/>
    public override void LinkEnter(
        BaseStackableChain chain,
        StackableChainValues values)
    {
        chain = chain ?? throw new ArgumentNullException(nameof(chain));
        values = values ?? throw new ArgumentNullException(nameof(values));

        Console.Write("|");
        Console.Write(chain.GetType().Name);
        Console.WriteLine();

        if (chain.InputKeys.Count <= 0)
        {
            return;
        }

        Console.Write("|");
        Console.Write("  ");
        Console.Write("\u2514");
        Console.Write("Input:");
        Console.WriteLine();
        foreach (var inputKey in chain.InputKeys)
        {
            Console.Write("|");
            Console.Write("   ");
            Console.Write("\u2514");
            var value = values.Value[inputKey];
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColorForKey(inputKey);
            Console.Write($" {inputKey}={ShortenString(value.ToString() ?? "", ValuesLength)}");
            Console.ForegroundColor = oldColor;
            Console.WriteLine();
        }
    }

    private readonly Dictionary<string, ConsoleColor> _colorMap = new();

    private ConsoleColor GetColorForKey(string key)
    {
        if (!UseColors)
            return Console.ForegroundColor;
        // if key is not in map, get unique color(except black and white)
        // if there no unique colors left, return white
        if (!_colorMap.TryGetValue(key, out var value))
        {
            var color = ConsoleColor.White;
#pragma warning disable CA2263
            var colors = Enum.GetValues(typeof(ConsoleColor));
#pragma warning restore CA2263
            foreach (ConsoleColor c in colors)
            {
                if (c == ConsoleColor.Black || c == ConsoleColor.White)
                    continue;
                if (!_colorMap.ContainsValue(c))
                {
                    color = c;
                    break;
                }
            }

            value = color;
            _colorMap.Add(key, value);
        }

        return value;
    }

    private static string ShortenString(string str, int length)
    {
        if (str.Length <= length)
        {
            return str;
        }

        return str[..(length - 3)] + "...";
    }

    /// <inheritdoc/>
    public override void LinkExit(
        BaseStackableChain chain,
        StackableChainValues values)
    {
        chain = chain ?? throw new ArgumentNullException(nameof(chain));
        values = values ?? throw new ArgumentNullException(nameof(values));

        if (chain.OutputKeys.Count <= 0)
        {
            return;
        }

        Console.Write("|");
        Console.Write("  ");
        Console.Write("\u2514");
        Console.Write("Output:");
        Console.WriteLine();

        foreach (var outputKey in chain.OutputKeys)
        {
            Console.Write("|");
            Console.Write("   ");
            Console.Write("\u2514");
            var value = values.Value[outputKey];
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColorForKey(outputKey);
            Console.Write($" {outputKey}={ShortenString(value.ToString() ?? "", ValuesLength)}");
            Console.ForegroundColor = oldColor;
            Console.WriteLine();
        }
    }
}