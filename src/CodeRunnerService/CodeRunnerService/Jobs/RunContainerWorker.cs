using System.Diagnostics;
using System.IO;
using CodeRunnerService.Jobs.Contracts;

namespace CodeRunnerService.Jobs
{
    public class RunContainerWorker: IRunContainerWorker
    {
        private readonly string filePath = @"/Users/alexturner/result";
        public string RunCodeInContainer(string fileName)
        {
            var runtimeConfig = new FileInfo(@"template.runtimeconfig.json");
            runtimeConfig.CopyTo(Path.Combine(filePath, $"{fileName}.runtimeconfig.json"));

            var dockerfile = new FileInfo(@"Dockerfile");
            dockerfile.CopyTo(Path.Combine(filePath, "Dockerfile"));
            
            var processInfo = new ProcessStartInfo();
            processInfo.FileName = "docker";
            var tagName = $"tezjarl/${fileName}:latest";
            processInfo.Arguments = $"build -t {tagName} .";
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            using (var buildProcess = Process.Start(processInfo))
            {
                buildProcess.Start();

                var buildError = buildProcess.StandardError.ReadToEnd();

                if (string.IsNullOrEmpty(buildError))
                {
                    processInfo.Arguments = $"run {tagName} {fileName}.dll";
                    using (var runningProcess = Process.Start(processInfo))
                    {
                        var runningError = runningProcess.StandardError.ReadToEnd();
                        var runningOutput = runningProcess.StandardOutput.ReadToEnd();
                        return string.IsNullOrEmpty(runningError) ? runningOutput : runningError;
                    }
                }

                return buildError;

            }
        }
    }
}