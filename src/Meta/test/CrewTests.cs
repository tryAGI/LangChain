using LangChain.Chains.StackableChains.Agents.Crew;
using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Anthropic;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class CrewTests
{
    [Test]
    public async Task can_test_crew()
    {
        // example app https://www.youtube.com/watch?v=sPzc6hMg7So

        var provider = new BedrockProvider();
        var llm = new Claude3SonnetModel(provider);

        const string origin = "New York";
        const string cities = "Kathmandu, Pokhara";
        const string dateRange = "May 13 to June 2, 2024";
        const string interests = "sight seeing, eating, tech";

        var myAgents = new Agents(llm);
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
        var provider = new BedrockProvider();
        var llm = new Claude3HaikuModel(provider);

        const string location = "Australia";
        const string cities = "Gold Coast, Sydney, Melbourne, Brisbane";
        const string dateRange = "May 13 to June 2, 2024";
        const string interests = "basketball, fishing";

        var prompt = $@"
i plan on vacationing in {location} and visiting {cities} during the {dateRange}.  these are my interests: {interests}.
";

        var myAgents = new Agents(llm);
        var agents = new List<CrewAgent>
        {
            myAgents.CityExpert,
            myAgents.LocalTourGuide
        };

        var chain =
            Set(prompt)
            | Crew(agents, myAgents.TravelAgent, inputKey: "text", outputKey: "text")
            | LLM(llm);

        Console.WriteLine(await chain.RunAsync("text"));
    }

    [Test]
    public async Task can_test_ReAct()
    {
        var provider = new BedrockProvider();
        var llm = new Claude3HaikuModel(provider);

        var googleKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? throw new InvalidOperationException("GOOGLE_API_KEY is not set");
        var googleCx = Environment.GetEnvironmentVariable("GOOGLE_API_CX") ?? throw new InvalidOperationException("GOOGLE_API_CX is not set");
        var searchTool = new GoogleCustomSearchTool(key: googleKey, cx: googleCx, resultsLimit: 1);

        var chain =
            Set("What is tryAGI/LangChain?")
            | ReActAgentExecutor(llm)
                .UseTool(searchTool);

        Console.WriteLine(await chain.RunAsync("text"));
    }
}