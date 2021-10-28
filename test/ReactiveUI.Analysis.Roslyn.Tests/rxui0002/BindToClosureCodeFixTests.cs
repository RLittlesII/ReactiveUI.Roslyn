using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.BindToClosureAnalyzer, ReactiveUI.Analysis.Roslyn.BindToClosureCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests.RXUI0002
{
    public class BindToClosureCodeFixTests : CSharpCodeFixTest<BindToClosureAnalyzer, BindToClosureCodeFixProvider, XUnitVerifier>
    {
        [Fact]
        public async Task GivenBindToClosure_WhenAnalyzed_ThenCodeFixed()
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(UnsupportedExpressionAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Error)
                    .WithSpan(12, 26, 12, 36)
                    .WithMessage(UnsupportedExpressionAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(BindToTestData.Incorrect, BindToTestData.Correct, diagnosticResult);
        }
    }
}