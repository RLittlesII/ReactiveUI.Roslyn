using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace ReactiveUI.Analyzers.Tests.Verifiers
{
    public static partial class AnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public class Test : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            public Test()
            {
                SolutionTransforms.Add((solution, projectId) =>
                {
                    var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                    compilationOptions =
                        compilationOptions
                            .WithSpecificDiagnosticOptions(compilationOptions.SpecificDiagnosticOptions.SetItems(VerifierHelper.NullableWarnings))
                            .WithMetadataImportOptions(MetadataImportOptions.Public);

                    var coreAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
                    var coreAssemblyNames = new[] {
                        "mscorlib.dll",
                        "netstandard.dll",
                        "System.dll",
                        "System.Core.dll",
#if NETCOREAPP
                        "System.Private.CoreLib.dll",
#endif
                        "System.Runtime.dll",
                    };
                    var coreMetaReferences = coreAssemblyNames.Select(x => MetadataReference.CreateFromFile(Path.Combine(coreAssemblyPath, x)));

                    var reactive = MetadataReference.CreateFromFile(typeof(Unit).Assembly.Location);
                    var reactiveui = MetadataReference.CreateFromFile(typeof(ReactiveCommand).Assembly.Location);
                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions)
                        .AddRuntimeLibrary(projectId, "netstandard.dll")
                        .AddRuntimeLibrary(projectId, "System.Runtime.dll")
                        // .AddRuntimeLibrary(projectId, coreAssemblyNames)
                        .AddMetadataReferences(projectId, coreMetaReferences)
                        .AddMetadataReference(projectId, reactive)
                        .AddMetadataReference(projectId, reactiveui);

                    return solution;
                });
            }
        }
    }

    public static partial class AnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        /// <inheritdoc cref="Microsoft.CodeAnalysis.Diagnostic"/>
        public static DiagnosticResult Diagnostic()
            => CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic();

        /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(string)"/>
        public static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic(diagnosticId);

        /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic(descriptor);

        /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new Test
            {
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected.AsEnumerable());
            await test.RunAsync(CancellationToken.None);
        }
    }
}