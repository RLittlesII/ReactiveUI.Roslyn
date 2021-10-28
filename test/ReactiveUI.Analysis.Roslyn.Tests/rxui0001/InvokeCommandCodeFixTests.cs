using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.InvokeCommandAnalyzer, ReactiveUI.Analysis.Roslyn.InvokeCommandCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests.RXUI0001
{
    public class InvokeCommandCodeFixTests : CSharpCodeFixTest<InvokeCommandAnalyzer, InvokeCommandCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [InlineData(InvokeCommandTestData.Incorrect, InvokeCommandTestData.Correct)]
        public async Task GivenNonExpressionInvokeCommand_WhenVerified_ThenCodeFixed(string incorrect, string correct)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(ExpressionLambdaAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(13, 32, 13, 39)
                   .WithMessage("Use expression lambda overload for property Command");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(incorrect, correct, diagnosticResult);
        }
    }
}