
using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Context;

public class ConsoleTraceHook: StackableChainHook
{
    public bool UseColors { get; set; }=true;
    public int ValuesLength { get; set; } = 40;
    public override void ChainStart(StackableChainValues values)
    {
        Console.WriteLine();
    }
    public override void LinkEnter(BaseStackableChain chain, StackableChainValues values)
    {

        Console.Write("|");
        Console.Write(chain.GetType().Name);
        Console.WriteLine();
        if (chain.InputKeys.Count > 0)
        {
            Console.Write("|");
            Console.Write("  ");
            Console.Write("\u2514");
            Console.Write("Input:");
            Console.WriteLine();
            foreach (string inputKey in chain.InputKeys)
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
        

    }

    Dictionary<string, ConsoleColor> _colorMap = new Dictionary<string, ConsoleColor>();

    ConsoleColor GetColorForKey(string key)
    {
        if(!UseColors)
            return Console.ForegroundColor;
        // if key is not in map, get unique color(except black and white)
        // if there no unique colors left, return white
        if (!_colorMap.ContainsKey(key))
        {
            var color = ConsoleColor.White;
            var colors = Enum.GetValues(typeof(ConsoleColor));
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
            _colorMap.Add(key, color);
        }
        return _colorMap[key];
    }

    string ShortenString(string str, int length)
    {
        if (str.Length <= length)
            return str;
        return str.Substring(0, length - 3) + "...";
    }

    public override void LinkExit(BaseStackableChain chain, StackableChainValues values)
    {
        if (chain.OutputKeys.Count > 0)
        {
            Console.Write("|");
            Console.Write("  ");
            Console.Write("\u2514");
            Console.Write("Output:");
            Console.WriteLine();
            foreach (string outputKey in chain.OutputKeys)
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
}