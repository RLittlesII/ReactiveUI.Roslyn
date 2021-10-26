using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Splat;

namespace ReactiveUI.Analyzers.Tests.Verifiers
{
    public static partial class CodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>
        {
            public Test()
            {
                SolutionTransforms.Add((solution, projectId) =>
                {
                    var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                    compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                        compilationOptions.SpecificDiagnosticOptions.SetItems(VerifierHelper.NullableWarnings));
                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

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
                    var splat = MetadataReference.CreateFromFile(typeof(IEnableLogger).Assembly.Location);
                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions)
                        .AddRuntimeLibrary(projectId, "netstandard.dll")
                        .AddRuntimeLibrary(projectId, "System.Runtime.dll")
                        .AddMetadataReferences(projectId, coreMetaReferences)
                        .AddMetadataReference(projectId, reactive)
                        .AddMetadataReference(projectId, reactiveui)
                       .AddMetadataReference(projectId, splat);

                    return solution;
                });
            }
        }
    }

    public static partial class CodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic()"/>
        public static DiagnosticResult Diagnostic()
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic();

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(string)"/>
        public static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(diagnosticId);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(descriptor);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static async Task VerifyAnalyzerAsync(string source, string fixedSource, params DiagnosticResult[] expected)
        {
            var test = new Test
            {
                TestCode = source,
                FixedCode = fixedSource
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, string)"/>
        public static async Task VerifyCodeFixAsync(string source, string fixedSource)
            => await VerifyCodeFixAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult, string)"/>
        public static async Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource)
            => await VerifyCodeFixAsync(source, new[] { expected }, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult[], string)"/>
        public static async Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource)
        {
            var test = new Test
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }
    }
}