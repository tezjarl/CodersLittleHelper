using System;

namespace CodeRunnerWebApi.Models
{
    public class CompilationResult
    {
        public bool IsCompilationSucceeded { get; set; }
        public string ErrorsList { get; set; } = String.Empty;
        public string AssemblyName { get; set; }
    }
}