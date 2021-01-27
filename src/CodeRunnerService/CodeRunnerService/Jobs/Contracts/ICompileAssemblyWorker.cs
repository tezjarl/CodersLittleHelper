using System.Collections.Generic;
using CodeRunnerService.Models;

namespace CodeRunnerService.Jobs.Contracts
{
    public interface ICompileAssemblyWorker
    {
        CompilationResult CompileAssembly(string codeToCompile, IEnumerable<string> usings, string assemblyName);
    }
}