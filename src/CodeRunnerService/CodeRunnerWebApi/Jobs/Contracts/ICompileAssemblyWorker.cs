using System.Collections.Generic;
using CodeRunnerWebApi.Models;

namespace CodeRunnerWebApi.Jobs.Contracts
{
    public interface ICompileAssemblyWorker
    {
        CompilationResult CompileAssembly(string codeToCompile, IEnumerable<string> usings, string assemblyName);
    }
}