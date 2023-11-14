using Docker.DotNet;
using Docker.DotNet.Models;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using System.Formats.Tar;

namespace LangChain.Extensions.Docker
{
    public class DockerChain : BaseStackableChain
    {
        class SuppressProgress : IProgress<JSONMessage>
        {
            public void Report(JSONMessage value)
            {

            }
        }

        private readonly DockerClient _client;
        public string Image { get; }
        public string Filename { get; }
        public string Command { get; }

        public DockerChain(string image= "python:3", string filename="main.py", string command="python", string inputKey="code",string outputKey="result")
        {
            Image = image;
            Filename = filename;
            Command = command;
            InputKeys = new[] {inputKey};
            OutputKeys = new[] {outputKey};

            _client = new DockerClientConfiguration()
                .CreateClient();
        }

        private string SanitizeCode(string code)
        {
            if (code.StartsWith("```"))
            {
                // remove first and last lines
                var lines = code.Split("\n");
                var res = string.Join("\n", lines[1..^1]);
                return res;
            }
            return code;
        }
        protected override async Task<IChainValues> InternalCall(IChainValues values)
        {

            await _client.Images.CreateImageAsync(new ImagesCreateParameters()
            {
                FromImage = Image
            }, null, new SuppressProgress(), CancellationToken.None).ConfigureAwait(false);


            var code = SanitizeCode(values.Value[InputKeys[0]].ToString());


            var tempDir = Path.GetTempPath();
            tempDir = Path.Combine(tempDir, Guid.NewGuid().ToString());
            var appDir = Path.Combine(tempDir, "app");
            Directory.CreateDirectory(appDir);
            var tempFile = Path.Combine(appDir, Filename);
            await File.WriteAllTextAsync(tempFile, code);

            MemoryStream archiveStream = new MemoryStream();
            await TarFile.CreateFromDirectoryAsync(tempDir,archiveStream,false);
            archiveStream.Seek(0, SeekOrigin.Begin);
            
            Directory.Delete(tempDir,true);

            var container = await _client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
               
                Image = Image,
                Cmd = new[] {Command,Filename},
                WorkingDir = "/app",
                
            }).ConfigureAwait(false);

            await _client.Containers.ExtractArchiveToContainerAsync(container.ID, new ContainerPathStatParameters()
            {
                AllowOverwriteDirWithFile = true,
                Path = "/",
            }, archiveStream, CancellationToken.None).ConfigureAwait(false);

            await _client.Containers.StartContainerAsync(container.ID, null).ConfigureAwait(false);

            await _client.Containers.WaitContainerAsync(container.ID).ConfigureAwait(false);

            var logs = await _client.Containers.GetContainerLogsAsync(container.ID,
                false,
                               new ContainerLogsParameters()
                               {
                    ShowStdout = true,
                    ShowStderr = true
                }).ConfigureAwait(false);

            var res = await logs.ReadOutputToEndAsync(CancellationToken.None).ConfigureAwait(false);

            var result = res.stdout+res.stderr;

            await _client.Containers.RemoveContainerAsync(container.ID,
                               new ContainerRemoveParameters()
                               {
                    Force = true
                }).ConfigureAwait(false);

            values.Value[OutputKeys[0]] = result;
            return values;
        }
    }
}