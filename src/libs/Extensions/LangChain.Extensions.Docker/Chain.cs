using LangChain.Chains.HelperChains;

namespace LangChain.Extensions.Docker;

/// <summary>
/// 
/// </summary>
public static class Chain
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    /// <param name="filename"></param>
    /// <param name="command"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static DockerChain RunCodeInDocker(string image= "python:3.10", string arguments = "main.py", string command = "python", string? attachVolume=null, string outputKey = "result")
    {
        return new DockerChain(image, arguments, command, attachVolume, outputKey);
    }
}