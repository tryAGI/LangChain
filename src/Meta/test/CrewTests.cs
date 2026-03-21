using LangChain.Chains.StackableChains.Agents.Crew;
using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;
using Microsoft.Extensions.AI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit("Requires ANTHROPIC_API_KEY")]
public class CrewTests
{
    private static IChatClient CreateChatClient()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") is { Length: > 0 } key ? key :
            throw new InconclusiveException("ANTHROPIC_API_KEY environment variable is not found.");

        return new Anthropic.AnthropicClient(apiKey);
    }

    [Test]
    public async Task can_test_crew()
    {
        IChatClient chatClient = CreateChatClient();

        const string origin = "New York";
        const string cities = "Kathmandu, Pokhara";
        const string dateRange = "May 13 to June 2, 2024";
        const string interests = "sight seeing, eating, tech";

        var myAgents = new Agents(chatClient);
        var agents = new List<CrewAgent>
        {
            myAgents.TravelAgent,
            myAgents.CityExpert,
            myAgents.LocalTourGuide
        };

        var agentTasks = new List<AgentTask>
        {
            Tasks.PlanItinerary(myAgents.TravelAgent, cities, dateRange, interests),
            Tasks.GatherCityInfo(myAgents.LocalTourGuide, cities, dateRange, interests),
            Tasks.IdentifyCity(myAgents.CityExpert, origin, cities, interests, dateRange)
        };

        var crew = new Crew(agents, agentTasks);
        var runAsync = await crew.RunAsync();
    }

    [Test]
    public async Task can_test_crewchain()
    {
        IChatClient chatClient = CreateChatClient();

        const string location = "Australia";
        const string cities = "Gold Coast, Sydney, Melbourne, Brisbane";
        const string dateRange = "May 13 to June 2, 2024";
        const string interests = "basketball, fishing";

        var prompt = $@"
i plan on vacationing in {location} and visiting {cities} during the {dateRange}.  these are my interests: {interests}.
";

        var myAgents = new Agents(chatClient);
        var agents = new List<CrewAgent>
        {
            myAgents.CityExpert,
            myAgents.LocalTourGuide
        };

        var chain =
            Set(prompt)
            | Crew(agents, myAgents.TravelAgent, inputKey: "text", outputKey: "text")
            | LLM(chatClient);

        Console.WriteLine(await chain.RunAsync("text"));
    }

    [Test]
    public async Task can_test_ReAct()
    {
        IChatClient chatClient = CreateChatClient();

        var googleKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? throw new InvalidOperationException("GOOGLE_API_KEY is not set");
        var googleCx = Environment.GetEnvironmentVariable("GOOGLE_API_CX") ?? throw new InvalidOperationException("GOOGLE_API_CX is not set");
        var searchTool = new GoogleCustomSearchTool(key: googleKey, cx: googleCx, resultsLimit: 1);

        var chain =
            Set("What is tryAGI/LangChain?")
            | ReActAgentExecutor(chatClient)
                .UseTool(searchTool);

        Console.WriteLine(await chain.RunAsync("text"));
    }
}
