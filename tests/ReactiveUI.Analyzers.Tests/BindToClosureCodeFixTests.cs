using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    ReactiveUI.Analyzers.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analyzers.BindToClosureAnalyzer, ReactiveUI.Analyzers.BindToClosureCodeFixProvider>;

namespace ReactiveUI.Analyzers.Tests
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