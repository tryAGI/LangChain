namespace LangChain.Extensions.Docker;

/// <summary>
/// 
/// </summary>
public static class Chain
{
    /// <summary>
    /// Executes code in a docker container
    /// </summary>
    /// <param name="image"></param>
    /// <param name="arguments"></param>
    /// <param name="command"></param>
    /// <param name="attachVolume"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static DockerChain RunCodeInDocker(
        string image= "python:3.10",
        string arguments = "main.py",
        string command = "python",
        string? attachVolume = null,
        string outputKey = "result")
    {
        return new DockerChain(image, arguments, command, attachVolume, outputKey);
    }
}