using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = RxUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<RxUI.Analysis.Roslyn.InvokeCommandAnalyzer, RxUI.Analysis.Roslyn.InvokeCommandCodeFixProvider>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0001
{
    public class InvokeCommandCodeFixTests : CSharpCodeFixTest<InvokeCommandAnalyzer, InvokeCommandCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [InlineData(InvokeCommandTestData.Incorrect, InvokeCommandTestData.Correct)]
        public async Task GivenNonExpressionInvokeCommand_WhenVerified_ThenCodeFixed(string incorrect, string correct)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(ExpressionLambdaOverloadAnalyzer.RXUI0001.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(13, 32, 13, 39)
                   .WithMessage("Use expression lambda overload for property Command");

            // When, Then
            await VerifyCS.VerifyCodeFixAsync(incorrect, correct, diagnosticResult);
        }
    }
}