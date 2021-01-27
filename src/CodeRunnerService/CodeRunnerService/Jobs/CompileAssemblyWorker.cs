using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRunnerService.Jobs.Contracts;
using CodeRunnerService.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeRunnerService.Jobs
{
    public class CompileAssemblyWorker: ICompileAssemblyWorker
    {
        private readonly LanguageVersion _languageVersion = LanguageVersion.Latest;

        private readonly string filePath = @"/Users/alexturner/result";

        public CompilationResult CompileAssembly(string codeToCompile, IEnumerable<string> usings, string assemblyName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile, new CSharpParseOptions(_languageVersion));
            
            var usingsList = usings.Select(u =>
            {
                var qualifiedName = SyntaxFactory.ParseName(u);
                return SyntaxFactory.UsingDirective(qualifiedName);
            });
            if (usingsList.Count() > 0)
            {
                syntaxTree = (syntaxTree.GetRoot() as CompilationUnitSyntax).AddUsings(usingsList.ToArray()).SyntaxTree;
            }

            var trustedAssembliesPath =
                ((string) AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            var references = trustedAssembliesPath
                .Select(a => MetadataReference.CreateFromFile(a))
                .ToList();

            var compilation = CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(
                    OutputKind.ConsoleApplication,
                    optimizationLevel: OptimizationLevel.Release))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);
            var emitResult = compilation.Emit(Path.Combine(filePath, $"{assemblyName}.dll"));
            return new CompilationResult
            {
                AssemblyName = assemblyName,
                IsCompilationSucceeded = emitResult.Success,
                ErrorsList = emitResult.Diagnostics.Aggregate(string.Empty,
                    (r, e) => r + e.GetMessage() + Environment.NewLine)
            };
        }
    }
}