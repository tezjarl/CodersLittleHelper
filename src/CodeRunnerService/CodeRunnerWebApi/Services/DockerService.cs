using System.Diagnostics;
using CodeRunnerWebApi.Services.Contracts;

namespace CodeRunnerWebApi.Services
{
    public class DockerService: IDockerService
    {
        
        public bool BuildImage(string assemblyName, string filePath)
        {
            var processInfo = CreateDefaultProcessStartInfo();
            processInfo.Arguments = $"build -t {CreateTagName(assemblyName)} {filePath}";
            using (var process = Process.Start(processInfo))
            {
                process.Start();
                var error = process.StandardError.ReadToEnd();
                return string.IsNullOrEmpty(error);
            }
        }

        public string RunContainer(string assemblyName)
        {
            var processInfo = CreateDefaultProcessStartInfo();
            processInfo.Arguments = $"run {CreateTagName(assemblyName)} {assemblyName}.dll";
            using (var process = Process.Start(processInfo))
            {
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                return string.IsNullOrEmpty(error) ? output : error;
            }
        }

        private string CreateTagName(string assemblyName) => $"tezjarl/${assemblyName}:latest";
        private ProcessStartInfo CreateDefaultProcessStartInfo() => new ProcessStartInfo
        {
            FileName = "docker",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
    }
}