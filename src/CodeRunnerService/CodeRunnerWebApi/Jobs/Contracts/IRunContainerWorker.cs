namespace CodeRunnerWebApi.Jobs.Contracts
{
    public interface IRunContainerWorker
    {
        string RunCodeInContainer(string fileName);
    }
}