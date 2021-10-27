using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.BindToClosureAnalyzer, ReactiveUI.Analysis.Roslyn.BindToClosureCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests
{
    public class BindToClosureCodeFixTests : CSharpCodeFixTest<BindToClosureAnalyzer, BindToClosureCodeFixProvider, XUnitVerifier>
    {
        [Fact]
        public async Task GivenDiagnosticResult_WhenAnalyzed_ThenCodeFixed()
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(BindToClosureAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Error)
                    .WithSpan(12, 26, 12, 36)
                    .WithMessage("Provide a well-formed lambda function .BindTo(this, x => x.Property)");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(BindToTestData.Incorrect, BindToTestData.Correct, diagnosticResult);
        }
    }
}