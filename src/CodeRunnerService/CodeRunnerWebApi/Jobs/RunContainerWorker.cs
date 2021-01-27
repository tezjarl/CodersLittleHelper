using System.Diagnostics;
using System.IO;
using CodeRunnerWebApi.Jobs.Contracts;
using CodeRunnerWebApi.Services;

namespace CodeRunnerWebApi.Jobs
{
    public class RunContainerWorker: IRunContainerWorker
    {
        private readonly string filePath = @"/Users/alexturner/result";
        public string RunCodeInContainer(string fileName)
        {
            CopyRuntimeConfig(fileName);
            CopyDockerfile();

            var dockerService = new DockerService();
            var isBuildSucceded = dockerService.BuildImage(fileName, filePath);
            if (!isBuildSucceded)
            {
                return "Build image failed";
            }

            var output = dockerService.RunContainer(fileName);
            return output;
        }

        private void CopyDockerfile()
        {
            var dockerfile = new FileInfo(@"Dockerfile");
            dockerfile.CopyTo(Path.Combine(filePath, "Dockerfile"));
        }

        private void CopyRuntimeConfig(string fileName)
        {
            var runtimeConfig = new FileInfo(@"template.runtimeconfig.json");
            runtimeConfig.CopyTo(Path.Combine(filePath, $"{fileName}.runtimeconfig.json"));
        }
    }
}