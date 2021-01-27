namespace CodeRunnerWebApi.Services.Contracts
{
    public interface IDockerService
    {
        bool BuildImage(string assemblyName, string filePath);
        string RunContainer(string assemblyName);
    }
}