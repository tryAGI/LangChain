using LangChain.Chains.HelperChains;

namespace LangChain.Extensions.Docker;

public static class Chain
{
    public static BaseStackableChain RunCodeInDocker(string image= "python:3.10", string filename = "main.py", string command = "python", string inputKey = "code", string outputKey = "result")
    {
        return new DockerChain(image, filename,command, inputKey, outputKey);
    }
}