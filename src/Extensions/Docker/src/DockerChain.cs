using Docker.DotNet;
using Docker.DotNet.Models;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;

namespace LangChain.Extensions.Docker
{
    /// <summary>
    /// 
    /// </summary>
    public class DockerChain : BaseStackableChain
    {
        sealed class SuppressProgress : IProgress<JSONMessage>
        {
            public void Report(JSONMessage value)
            {

            }
        }

        private readonly DockerClient _client;
        
        /// <summary>
        /// 
        /// </summary>
        public string Image { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Arguments { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Command { get; }

        public string? AttachVolume { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="arguments"></param>
        /// <param name="command"></param>
        /// <param name="attachVolume"></param>
        /// <param name="outputKey"></param>
        public DockerChain(
            string image= "python:3",
            string arguments="main.py",
            string command="python",
            string? attachVolume=null,
            string outputKey="result")
        {
            Image = image;
            Arguments = arguments;
            Command = command;
            AttachVolume = attachVolume;
            OutputKeys = new[] {outputKey};

            using var configuration = new DockerClientConfiguration();
            _client = configuration.CreateClient();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<IChainValues> InternalCallAsync(IChainValues values, CancellationToken cancellationToken = default)
        {
            values = values ?? throw new ArgumentNullException(nameof(values));
            
            await _client.Images.CreateImageAsync(new ImagesCreateParameters()
            {
                FromImage = Image
            }, null, new SuppressProgress(), CancellationToken.None).ConfigureAwait(false);

            var binds = new List<string>();

            
            if (AttachVolume != null)
            {
                var absolutePath = Path.GetFullPath(AttachVolume)
                    .Replace("\\","/")
                    .Replace(":","");
                binds.Add($"/{absolutePath}:/app");
            }

            var container = await _client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
               
                Image = Image,
                Cmd = new[] {Command,Arguments},
                WorkingDir = "/app",
                HostConfig = new HostConfig()
                {
                    Binds = binds
                }
                
            }, cancellationToken).ConfigureAwait(false);



            await _client.Containers.StartContainerAsync(container.ID, null, cancellationToken).ConfigureAwait(false);

            await _client.Containers.WaitContainerAsync(container.ID, cancellationToken).ConfigureAwait(false);

            var logs = await _client.Containers.GetContainerLogsAsync(container.ID,
                false,
                               new ContainerLogsParameters()
                               {
                    ShowStdout = true,
                    ShowStderr = true
                }, cancellationToken).ConfigureAwait(false);

            var res = await logs.ReadOutputToEndAsync(CancellationToken.None).ConfigureAwait(false);

            var result = res.stdout+res.stderr;

            await _client.Containers.RemoveContainerAsync(container.ID,
                               new ContainerRemoveParameters()
                               {
                    Force = true
                }, cancellationToken).ConfigureAwait(false);

            values.Value[OutputKeys[0]] = result;
            return values;
        }
    }
}