using System.Data;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;

namespace LangChain.IntegrationTests;

public class CalculatorTool()
    : CrewAgentTool("calculate", "Useful to perform any math calculation like sum, minus, multiplication, division, etc." +
                                 "The input to this tool should be mathematical expression. " +
                                 "For example: '2 + 2' or '3 * 4'.)"
    )
{
    public override Task<string> ToolTask(string operation, CancellationToken token = default)
    {
        try
        {
            var dt = new DataTable();
            var compute = dt.Compute(operation, "");

            return Task.FromResult(compute.ToString()!);
        }
        catch (SyntaxErrorException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}