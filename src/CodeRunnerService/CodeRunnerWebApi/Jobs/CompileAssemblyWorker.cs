using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRunnerWebApi.Jobs.Contracts;
using CodeRunnerWebApi.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeRunnerWebApi.Jobs
{
    public class CompileAssemblyWorker: ICompileAssemblyWorker
    {
        private const string TrustedPlatformAssemblies = "TRUSTED_PLATFORM_ASSEMBLIES";
        private readonly LanguageVersion _languageVersion = LanguageVersion.Latest;

        private readonly string filePath = @"/Users/alexturner/result";

        public CompilationResult CompileAssembly(string codeToCompile, IEnumerable<string> usings, string assemblyName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile, new CSharpParseOptions(_languageVersion));
            
            syntaxTree = AddUsingsToSyntaxTree(usings, syntaxTree);

            var references = GetReferencesList();

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

        private List<PortableExecutableReference> GetReferencesList()
        {
            var trustedAssembliesPath =
                ((string) AppContext.GetData(TrustedPlatformAssemblies)).Split(Path.PathSeparator);

            return trustedAssembliesPath
                .Select(a => MetadataReference.CreateFromFile(a))
                .ToList();
        }

        private SyntaxTree AddUsingsToSyntaxTree(IEnumerable<string> usings, SyntaxTree syntaxTree)
        {
            var usingsList = usings.Select(u =>
            {
                var qualifiedName = SyntaxFactory.ParseName(u);
                return SyntaxFactory.UsingDirective(qualifiedName);
            });
            if (usingsList.Count() > 0)
            {
                syntaxTree = (syntaxTree.GetRoot() as CompilationUnitSyntax).AddUsings(usingsList.ToArray()).SyntaxTree;
            }

            return syntaxTree;
        }
    }
}