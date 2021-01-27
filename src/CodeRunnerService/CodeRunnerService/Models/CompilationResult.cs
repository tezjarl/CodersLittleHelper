using System;

namespace CodeRunnerService.Models
{
    public class CompilationResult
    {
        public bool IsCompilationSucceeded { get; set; }
        public string ErrorsList { get; set; } = String.Empty;
        public string AssemblyName { get; set; }
    }
}