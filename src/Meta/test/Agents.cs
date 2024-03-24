using LangChain.Chains.StackableChains.Agents.Crew;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;
using LangChain.Providers;

namespace LangChain.IntegrationTests;

//Creating Agents Cheat Sheet:
//- Think like a boss.Work backwards from the goal and think which employee
//    you need to hire to get the job done.
//- Define the Captain of the crew who orient the other agents towards the goal. 
//- Define which experts the captain needs to communicate with and delegate tasks to.
//Build a top down structure of the crew.

//  Goal:
//  - Create a 7-day travel itinerary with detailed per-day plans,
//      including budget, packing suggestions, and safety tips.

//  Captain/Manager/Boss:
//  - Expert Travel Agent

//  Employees/Experts to hire:
//  - City Selection Expert
//  - Local Tour Guide


//  Notes:
//  - Agents should be results driven and have a clear goal in mind
//  - Role is their job title
//  - Goals should actionable
//  - Backstory should be their resume

public class Agents
{
    public Agents(IChatModel model)
    {
        var googleKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        var googleCx = Environment.GetEnvironmentVariable("GOOGLE_API_CX");

        TravelAgent = new CrewAgent(
            model: model,
            role: "Expert Travel Agent",
            goal: "Create a 7-day travel itinerary with detailed per-day plans, including budget, packing suggestions, and safety tips.",
            backstory: "I am a travel agent with a passion for exploring new destinations and sharing my knowledge with others.  " +
                       "Expert in the art of planning and organizing trips, and I am always eager to learn and grow as a professional."
        );
        TravelAgent.AddTools(new List<CrewAgentTool>
        {
            new GoogleSearchTool(googleKey, googleCx),
            new CalculatorTool()
        });

        CityExpert = new CrewAgent(
            model: model,
            role: "City Selection Expert",
            goal: "Select the best cities based on weather, season, prices and travelers interests",
            backstory: "Expert at analyzing travel data to pick ideal destinations for travelers"
        );
        CityExpert.AddTools(new List<CrewAgentTool>
        {
            new GoogleSearchTool(googleKey, googleCx),
        });

        LocalTourGuide = new CrewAgent(
            model: model,
            role: "Local Tour Guide",
            goal: "Provide the BEST insights about the selected city and its attractions",
            backstory: "I am a local tour guide with expertise in providing detailed information about cities and their attractions."
        );
        LocalTourGuide.AddTools(new List<CrewAgentTool>
        {
            new GoogleSearchTool(googleKey, googleCx),
        });
    }

    public CrewAgent TravelAgent { get; set; }
    public CrewAgent CityExpert { get; set; }
    public CrewAgent LocalTourGuide { get; set; }
}