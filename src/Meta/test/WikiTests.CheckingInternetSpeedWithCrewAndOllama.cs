using System.Diagnostics;
using System.Net;
using LangChain.Chains.StackableChains.Agents.Crew;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using Ollama;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

public partial class WikiTests
{
    [Test]
    public async Task CheckingInternetSpeedWithCrewAndOllama()
    {
        //// # Welcome to the crew
        //// Crew is a combination of different agents with different set of skills.
        //// This is usefull when making big and capable systems. Since one agent can't do everything,
        //// the crew approach allows to distribure load across different agents.  
        //// Even if you are not building a complex system you can apply the crew approach to local models.
        //// Since they are quite limited they are not able to perform more than one or two tasks sucessfully.  
        //// 
        //// ## The task
        //// Lets take a simple task to get current internet speed using windows built-in ping tool.  
        //// 
        //// # Setup
        //// We will setup `mistral` ollama model, but you can easily switch to any other model or provider.

        var provider = new OllamaProvider(
            // url: "http://172.16.50.107:11434", // if you have ollama running on different computer/port. Default is "http://localhost:11434/api"
            options: new RequestOptions
            {
                Temperature = 0,
            });
        var model = new OllamaChatModel(provider, id: "llama3.1").UseConsoleForDebug();

        //// ## Making a tool
        //// 
        //// One of our agents would need to have access to windows ping tool. So let's build it!

        var pingTool = new CrewAgentToolLambda("ping", "executes ping on specified ip address", address =>
        {
            var addressParsed = IPAddress.Parse(address);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ping",
                    Arguments = "-n 5 "+ addressParsed,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return Task.FromResult(result);
        });
        
        //// Nothing special here. We just parse ip-address and executing ping command with it.
        //// Then we are taking the results and returning them as an answer.
        //// 
        //// ## Assembling the crew
        //// We would need 2 agents:  
        ////  - manager - every crew should have one. he is controlling and delegating tasks between agents  
        ////  - pinger - our `ping` command specialist. He came here to chew bubble gum and ping servers, and he's all out of bubble gum.  

        // controls agents
        var manager = new CrewAgent(model,"manager","assign task to one of your co-workers and return the result");

        // the actual agent who does the job
        var pinger = new CrewAgent(model,"pinger","checks the speed of internet connection",
            "you using ping command and analyzing it's result. After this you print a single number as your final answer(the connection speed in milliseconds).");
        
        //// Notice that we are giving a backstory to the `pinger` so he would understand his job better.
        //// 
        //// Now let's give a tool to out pinger.
        
        pinger.AddTools(new []{pingTool});
        
        //// Now this tool is assigned to pinger and only he has access to it.
        //// 
        //// ## Building a chain
        //// 
        //// As always we are finishing by building a chain:
        
        var chain = 
            Set("What is my ping to google's main dns server?")
            | Crew(new []{manager,pinger},manager);

        var res = await chain.RunAsync("text");
        Console.WriteLine(res);
        
        //// Crew chain requires a full list of agens(inluding manager) and manager agent specified separately.
        //// 
        //// # Lets run and test it:
        //// 
        //// As the first step the task for manager is created asking a question:
        //// 
        //// _What is my ping to google's main dns server?_
        //// 
        //// As you may expect `google's main dns server` is a common knowladge, so manager aslo knows that it is `8.8.8.8`.  
        //// Now he needs a way to ping it. But he can't do this by himself. So he asks the pinger to do his job:
        //// ```
        //// ...
        //// Action: question
        //// Action Input: pinger|What is the current ping time to Google's main DNS server (8.8.8.8)?
        //// ...
        //// ```
        //// Now it's pinger's part. He sees that he has `ping` it his toolbelt so he can use it:
        //// ```
        //// ...
        //// Action: ping
        //// Action Input: 8.8.8.8
        //// ...
        //// Final Answer: The average ping time to Google's main DNS server (8.8.8.8) is 17 milliseconds.
        //// ```
        //// 
        //// Now manager receives the answer and makes the final conclusion:
        //// ```
        //// ...
        //// Observation: The average ping time to Google's main DNS server (8.8.8.8) is 17 milliseconds.
        //// Thought: Do I need to use a tool? No
        //// Final Answer: The current ping time to Google's main DNS server is 17 milliseconds.
        //// ```
    }
}